using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoInvestimentos.Data;
using ProjetoInvestimentos.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Npgsql;

namespace ProjetoInvestimentos.Controllers;

/// <summary>
/// Gerenciamento de usuários - SUPER SIMPLES
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("👥 USUÁRIOS - Cadastro simples (CPF + Nome)")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _connectionString;

    public UsuariosController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    /// <summary>
    /// 🚀 GUIA RÁPIDO - Como usar usuários
    /// </summary>
    /// <returns>Instruções simples de uso</returns>
    [HttpGet("ajuda")]
    [SwaggerOperation(
        Summary = "🚀 AJUDA - Como usar usuários",
        Description = "Clique aqui primeiro! Mostra um guia simples de como gerenciar usuários nesta API"
    )]
    [SwaggerResponse(200, "Guia de uso de usuários")]
    public ActionResult GetHelp()
    {
        var help = new
        {
            Titulo = "👥 GUIA RÁPIDO - API de Usuários",
            EsquemaDB = "Nome fica no campo 'dados' como JSON: {\"nome\": \"João Silva\"}",
            ComoUsar = new
            {
                CriarUsuario = "POST /api/usuarios - Só precisa de: CPF (11 números) + Nome + Email(opcional)",
                VerTodos = "GET /api/usuarios - Lista TODOS os usuários com nomes extraídos do JSON",
                VerPorCPF = "GET /api/usuarios/{cpf} - Ver dados de uma pessoa específica",
                EditarUsuario = "PUT /api/usuarios/{cpf} - Atualizar nome e email (CPF na URL)",
                DeletarUsuario = "DELETE /api/usuarios/{cpf} - Remove se não tiver investimentos"
            },
            ExemploSimples = new
            {
                CPF = "12345678901",
                Nome = "João Silva Santos",
                Email = "joao.silva@email.com"
            },
            CampoJSON = "O nome é salvo assim: dados = {\"nome\": \"João Silva Santos\"}",
            Dica = "💡 A API extrai automaticamente o nome do JSON ao exibir usuários!"
        };
        return Ok(help);
    }

    /// <summary>
    /// 👥 Lista todos os usuários cadastrados (LINQ)
    /// </summary>
    /// <returns>Lista de usuários com nomes extraídos do JSON</returns>
    /// <response code="200">Lista de usuários retornada com sucesso</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "👥 Lista TODOS os usuários [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa OrderBy() para ordenar por data de criação. Retorna lista completa de usuários com nomes extraídos do JSON automaticamente"
    )]
    [SwaggerResponse(200, "Lista de usuários retornada com sucesso", typeof(IEnumerable<UserProfile>))]
    public async Task<ActionResult<IEnumerable<UserProfile>>> GetAll()
    {
        // LINQ: Ordenação por data de criação
        var usuarios = await _context.UserProfiles
            .OrderBy(u => u.CriadoEm)
            .ToListAsync();

        // Extrair nomes do campo JSON dados
        foreach (var usuario in usuarios)
        {
            usuario.Nome = ExtractNomeFromDados(usuario.Dados);
        }

        return Ok(usuarios);
    }

    /// <summary>
    /// 🔍 Busca um usuário pelo CPF (LINQ)
    /// </summary>
    /// <param name="cpf">CPF do usuário (apenas números, 11 dígitos)</param>
    /// <returns>Dados do usuário com nome extraído do JSON</returns>
    /// <response code="200">Usuário encontrado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("{cpf}")]
    [SwaggerOperation(
        Summary = "🔍 Buscar usuário por CPF [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa FirstOrDefaultAsync() para buscar por CPF específico. Digite apenas números do CPF, sem pontos ou traços."
    )]
    [SwaggerResponse(200, "Usuário encontrado", typeof(UserProfile))]
    [SwaggerResponse(404, "Usuário não encontrado")]
    public async Task<ActionResult<UserProfile>> GetByCpf(
        [FromRoute, SwaggerParameter("CPF do usuário (apenas números, exemplo: 12345678901)", Required = true)] string cpf)
    {
        // LINQ: Busca usuário por CPF
        var usuario = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Cpf == cpf);
        
        if (usuario == null)
            return NotFound($"Usuário com CPF {cpf} não encontrado");
        
        // Extrair nome do campo JSON dados
        usuario.Nome = ExtractNomeFromDados(usuario.Dados);
        
        return Ok(usuario);
    }

    /// <summary>
    /// Extrai o nome do campo JSON dados
    /// </summary>
    /// <param name="dados">String JSON contendo os dados</param>
    /// <returns>Nome extraído ou null se não encontrado</returns>
    private string? ExtractNomeFromDados(string? dados)
    {
        if (string.IsNullOrEmpty(dados))
            return null;

        try
        {
            using var document = JsonDocument.Parse(dados);
            if (document.RootElement.TryGetProperty("nome", out var nomeElement))
            {
                return nomeElement.GetString();
            }
        }
        catch
        {
            // Se houver erro na deserialização, retornar null
        }

        return null;
    }

    /// <summary>
    /// 🆕 Cria um novo usuário
    /// </summary>
    /// <param name="request">Dados do usuário a ser criado</param>
    /// <returns>Usuário criado</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos ou CPF já existe</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "🆕 CRIAR novo usuário",
        Description = "Cadastra um novo usuário no sistema. Use CPF apenas com números (11 dígitos). O nome vai automaticamente para o campo dados JSON. Email é opcional."
    )]
    [SwaggerResponse(201, "Usuário criado com sucesso", typeof(UserProfile))]
    [SwaggerResponse(400, "Dados inválidos ou CPF já existe")]
    public async Task<ActionResult<UserProfile>> Create(
        [FromBody, SwaggerRequestBody("Dados do usuário", Required = true)] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar nome obrigatório
            if (string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest("Nome é obrigatório");

            // Usar SQL direto para inserção direta
            var agora = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var dados = System.Text.Json.JsonSerializer.Serialize(new { nome = request.Nome.Trim() });

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Primeiro verificar se o CPF já existe
            using var checkCommand = new NpgsqlCommand(
                "SELECT COUNT(1) FROM public.user_profiles WHERE cpf = @cpf", 
                connection);
            checkCommand.Parameters.AddWithValue("@cpf", request.Cpf);
            
            var result = await checkCommand.ExecuteScalarAsync();
            var exists = result != null && (long)result > 0;
            if (exists)
                return BadRequest($"Usuário com CPF {request.Cpf} já existe");

            // Inserir o novo usuário
            using var insertCommand = new NpgsqlCommand(
                "INSERT INTO public.user_profiles (cpf, email, dados, criado_em, alterado_em) VALUES (@cpf, @email, @dados::jsonb, @criado_em, @alterado_em) RETURNING id", 
                connection);
                
            insertCommand.Parameters.AddWithValue("@cpf", request.Cpf);
            insertCommand.Parameters.AddWithValue("@email", request.Email ?? (object)DBNull.Value);
            insertCommand.Parameters.AddWithValue("@dados", dados ?? (object)DBNull.Value);
            insertCommand.Parameters.AddWithValue("@criado_em", agora);
            insertCommand.Parameters.AddWithValue("@alterado_em", agora);

            var userId = await insertCommand.ExecuteScalarAsync();
            if (userId == null)
                return StatusCode(500, "Erro ao criar usuário");

            var usuario = new UserProfile
            {
                Id = (Guid)userId,
                Cpf = request.Cpf,
                Email = request.Email,
                Dados = dados,
                CriadoEm = agora,
                AlteradoEm = agora,
                Nome = request.Nome
            };

            return CreatedAtAction(nameof(GetByCpf), new { cpf = usuario.Cpf }, usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    /// <summary>
    /// ✏️ Atualiza os dados de um usuário
    /// </summary>
    /// <param name="cpf">CPF do usuário</param>
    /// <param name="request">Novos dados do usuário</param>
    /// <returns>Usuário atualizado</returns>
    /// <response code="200">Usuário atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPut("{cpf}")]
    [SwaggerOperation(
        Summary = "✏️ ATUALIZAR usuário",
        Description = "Modifica o nome (salvo no campo dados JSON) e email de um usuário existente. CPF não pode ser alterado."
    )]
    [SwaggerResponse(200, "Usuário atualizado com sucesso", typeof(UserProfile))]
    [SwaggerResponse(400, "Dados inválidos")]
    [SwaggerResponse(404, "Usuário não encontrado")]
    public async Task<ActionResult<UserProfile>> Update(
        [FromRoute, SwaggerParameter("CPF do usuário", Required = true)] string cpf,
        [FromBody, SwaggerRequestBody("Dados atualizados do usuário", Required = true)] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest("Nome é obrigatório");

        // LINQ: Busca usuário para atualização
        var usuario = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Cpf == cpf);
        
        if (usuario == null)
            return NotFound($"Usuário com CPF {cpf} não encontrado");

        try
        {
            // Atualizar email se fornecido
            if (!string.IsNullOrEmpty(request.Email))
            {
                usuario.Email = request.Email;
            }

            // Atualizar dados JSON com o novo nome
            var dados = new Dictionary<string, object>();
            
            // Se já existe dados JSON, preservar outros campos
            if (!string.IsNullOrEmpty(usuario.Dados))
            {
                try
                {
                    var existingDados = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(usuario.Dados);
                    if (existingDados != null)
                    {
                        dados = existingDados;
                    }
                }
                catch
                {
                    // Se houver erro na deserialização, criar novo dicionário
                    dados = new Dictionary<string, object>();
                }
            }
            
            // Atualizar nome
            dados["nome"] = request.Nome;
            
            // Serializar de volta
            usuario.Dados = System.Text.Json.JsonSerializer.Serialize(dados, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            usuario.AlteradoEm = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            
            await _context.SaveChangesAsync();
            
            // Definir Nome para retorno
            usuario.Nome = request.Nome;
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar usuário: {ex.Message}");
        }
        
        return Ok(usuario);
    }

    /// <summary>
    /// 🗑️ Remove um usuário (LINQ)
    /// </summary>
    /// <param name="cpf">CPF do usuário a ser removido</param>
    /// <returns>Confirmação de remoção</returns>
    /// <response code="204">Usuário removido com sucesso</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="400">Usuário possui investimentos e não pode ser removido</response>
    [HttpDelete("{cpf}")]
    [SwaggerOperation(
        Summary = "🗑️ DELETAR usuário [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa FirstOrDefaultAsync() + AnyAsync() para buscar usuário e verificar investimentos. ⚠️ ATENÇÃO: Exclusão permanente!"
    )]
    [SwaggerResponse(204, "Usuário removido com sucesso")]
    [SwaggerResponse(404, "Usuário não encontrado")]
    [SwaggerResponse(400, "Usuário possui investimentos e não pode ser removido")]
    public async Task<ActionResult> Delete(
        [FromRoute, SwaggerParameter("CPF do usuário a ser removido", Required = true)] string cpf)
    {
        // LINQ: Busca usuário para exclusão
        var usuario = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Cpf == cpf);
        
        if (usuario == null)
            return NotFound($"Usuário com CPF {cpf} não encontrado");

        // LINQ: Verifica se o usuário possui investimentos
        var hasInvestments = await _context.Investimentos
            .AnyAsync(i => i.UserId == usuario.Id);
        
        if (hasInvestments)
            return BadRequest("Não é possível remover um usuário que possui investimentos. Remova os investimentos primeiro.");

        _context.UserProfiles.Remove(usuario);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}

/// <summary>
/// 🆕 Dados para criar usuário SIMPLIFICADO
/// </summary>
[SwaggerSchema("Só precisa de CPF e Nome! (Email opcional)")]
public class CreateUserRequest
{
    /// <summary>
    /// CPF do usuário (apenas números, sem pontos ou traços)
    /// </summary>
    [Required(ErrorMessage = "CPF é obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 dígitos")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números")]
    [SwaggerSchema("CPF: apenas os 11 números. Ex: 12345678901")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário (será salvo no campo dados JSON)
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [SwaggerSchema("Nome completo. Ex: João Silva Santos")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário (opcional)
    /// </summary>
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [SwaggerSchema("Email (opcional). Ex: joao.silva@email.com")]
    public string? Email { get; set; }
}

/// <summary>
/// ✏️ Dados para atualizar usuário SIMPLIFICADO
/// </summary>
[SwaggerSchema("Atualiza nome (no JSON) e email. CPF não muda!")]
public class UpdateUserRequest
{
    /// <summary>
    /// Nome do usuário (será salvo no campo dados JSON)
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    [SwaggerSchema("Nome completo (salvo no campo JSON dados)")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário (opcional)
    /// </summary>
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [StringLength(254, ErrorMessage = "Email deve ter no máximo 254 caracteres")]
    [SwaggerSchema("Email (opcional)")]
    public string? Email { get; set; }
}