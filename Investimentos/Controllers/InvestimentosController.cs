using Microsoft.AspNetCore.Mvc;
using ProjetoInvestimentos.Models;
using ProjetoInvestimentos.Repositories;
using ProjetoInvestimentos.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ProjetoInvestimentos.Controllers;

/// <summary>
/// Controller de investimentos - API SUPER SIMPLIFICADA
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("💰 INVESTIMENTOS - API Simplificada (só CPF + dados básicos!)")]
public class InvestimentosController : ControllerBase
{
    private readonly IInvestimentoRepository _repository;

    public InvestimentosController(IInvestimentoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 🚀 GUIA RÁPIDO - Como usar a API
    /// </summary>
    /// <returns>Instruções simples de uso</returns>
    [HttpGet("ajuda")]
    [SwaggerOperation(
        Summary = "🚀 AJUDA - Como usar a API",
        Description = "Clique aqui primeiro! Mostra um guia simples de como usar todos os endpoints desta API de forma fácil"
    )]
    [SwaggerResponse(200, "Guia de uso da API")]
    public ActionResult GetHelp()
    {
        var help = new
        {
            Titulo = "🎯 GUIA RÁPIDO - API de Investimentos",
            ComoUsar = new
            {
                CriarInvestimento = "POST /api/investimentos - Só precisa de: CPF (11 números), tipo, código, valor e operação (compra/venda)",
                VerTodos = "GET /api/investimentos - Lista TODOS os investimentos",
                VerPorCPF = "GET /api/investimentos/usuario/{cpf} - Ver investimentos de uma pessoa",
                EditarInvestimento = "PUT /api/investimentos/{id} - Só o ID vai na URL, dados no body",
                DeletarInvestimento = "DELETE /api/investimentos/{id} - Remove permanentemente",
                CalcularSaldo = "GET /api/investimentos/saldo/{cpf} - Quanto a pessoa tem líquido",
                Dashboard = "GET /api/investimentos/dashboard - Resumo geral por tipo"
            },
            ExemploSimples = new
            {
                CPF = "12345678901",
                Tipo = "Ação",
                Codigo = "PETR4",
                Valor = 1500.50m,
                Operacao = "compra"
            },
            Dica = "💡 A API gera automaticamente IDs, datas e busca usuários pelo CPF!"
        };
        return Ok(help);
    }

    // CRUD completo (35%)
    
    /// <summary>
    /// 📋 Lista todos os investimentos - mais recentes primeiro (LINQ)
    /// </summary>
    /// <returns>Lista de todos os investimentos ordenados por data</returns>
    /// <response code="200">Lista completa de investimentos</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "📋 Lista TODOS os investimentos [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa OrderByDescending() para ordenar por data. Visualize toda a base de dados de investimentos (mais recentes primeiro)."
    )]
    [SwaggerResponse(200, "Lista de investimentos retornada com sucesso", typeof(IEnumerable<Investimento>))]
    public async Task<ActionResult<IEnumerable<Investimento>>> GetAll()
    {
        var investimentos = await _repository.GetAllAsync();
        return Ok(investimentos);
    }

    /// <summary>
    /// 🔍 Busca um investimento específico pelo ID (LINQ)
    /// </summary>
    /// <param name="id">ID único do investimento</param>
    /// <returns>Dados completos do investimento</returns>
    /// <response code="200">Investimento encontrado</response>
    /// <response code="404">Investimento não encontrado</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "🔍 Busca investimento por ID [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa FirstOrDefaultAsync() para buscar por ID específico. Cole o ID de qualquer investimento para ver detalhes completos."
    )]
    [SwaggerResponse(200, "Investimento encontrado", typeof(Investimento))]
    [SwaggerResponse(404, "Investimento não encontrado")]
    public async Task<ActionResult<Investimento>> GetById(
        [FromRoute, SwaggerParameter("ID único do investimento", Required = true)] Guid id)
    {
        var investimento = await _repository.GetByIdAsync(id);
        if (investimento == null)
            return NotFound();
        
        return Ok(investimento);
    }

    /// <summary>
    /// 👤 Lista todos os investimentos de um usuário (LINQ)
    /// </summary>
    /// <param name="userCpf">CPF do usuário (apenas números, 11 dígitos)</param>
    /// <returns>Todos os investimentos do usuário</returns>
    /// <response code="200">Lista de investimentos do usuário</response>
    [HttpGet("usuario/{userCpf}")]
    [SwaggerOperation(
        Summary = "👤 Meus investimentos por CPF [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa Where() + OrderByDescending() para filtrar por CPF. Digite apenas os NÚMEROS do CPF para ver todos os investimentos da pessoa."
    )]
    [SwaggerResponse(200, "Lista de investimentos do usuário", typeof(IEnumerable<Investimento>))]
    public async Task<ActionResult<IEnumerable<Investimento>>> GetByUserCpf(
        [FromRoute, SwaggerParameter("CPF apenas números (exemplo: 12345678901)", Required = true)] string userCpf)
    {
        var investimentos = await _repository.GetByUserCpfAsync(userCpf);
        return Ok(investimentos);
    }

    /// <summary>
    /// Cria um novo investimento (SIMPLIFICADO)
    /// </summary>
    /// <param name="request">Dados simplificados do investimento</param>
    /// <returns>Investimento criado com ID gerado</returns>
    /// <response code="201">Investimento criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "🚀 Cria investimento (APENAS CPF + dados básicos)",
        Description = "Cadastra um novo investimento de forma simplificada. Informe apenas: CPF (11 dígitos), tipo, código, valor e operação. IDs e datas são gerados automaticamente!"
    )]
    [SwaggerResponse(201, "Investimento criado com sucesso", typeof(Investimento))]
    [SwaggerResponse(400, "Dados de entrada inválidos")]
    public async Task<ActionResult<Investimento>> Create(
        [FromBody, SwaggerRequestBody("Dados simplificados do investimento", Required = true)] CreateInvestimentoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validações adicionais
        if (request.Operacao.ToLower() != "compra" && request.Operacao.ToLower() != "venda")
        {
            return BadRequest("Operação deve ser 'compra' ou 'venda'");
        }

        try
        {
            // Criar objeto Investimento a partir do DTO
            var investimento = new Investimento
            {
                UserCpf = request.UserCpf.Trim(),
                Tipo = request.Tipo.Trim(),
                Codigo = request.Codigo.Trim().ToUpper(),
                Valor = request.Valor,
                Operacao = request.Operacao.Trim().ToLower(),
                // Id, UserId, CriadoEm e AlteradoEm serão definidos no repositório
            };

            var created = await _repository.CreateAsync(investimento);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Dados inválidos: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"Erro de operação: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Log do erro completo (em produção, usar ILogger)
            var fullError = ex.InnerException != null ? 
                $"{ex.Message} | Inner: {ex.InnerException.Message}" : 
                ex.Message;
            
            return BadRequest($"Erro ao criar investimento: {fullError}");
        }
    }

    /// <summary>
    /// Atualiza um investimento existente (SIMPLIFICADO)
    /// </summary>
    /// <param name="id">ID do investimento a ser atualizado</param>
    /// <param name="request">Novos dados do investimento (apenas campos editáveis)</param>
    /// <returns>Investimento atualizado</returns>
    /// <response code="200">Investimento atualizado com sucesso</response>
    /// <response code="400">ID não confere ou dados inválidos</response>
    /// <response code="404">Investimento não encontrado</response>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "🔄 Atualiza investimento (ID na URL, dados no body)",
        Description = "Modifica apenas os dados editáveis: tipo, código, valor e operação. O ID vem na URL. CPF, datas e IDs internos são preservados automaticamente!"
    )]
    [SwaggerResponse(200, "Investimento atualizado com sucesso", typeof(Investimento))]
    [SwaggerResponse(400, "ID não confere ou dados inválidos")]
    [SwaggerResponse(404, "Investimento não encontrado")]
    public async Task<ActionResult<Investimento>> Update(
        [FromRoute, SwaggerParameter("ID do investimento", Required = true)] Guid id, 
        [FromBody, SwaggerRequestBody("Dados simplificados para atualização", Required = true)] UpdateInvestimentoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validações adicionais
        if (request.Operacao.ToLower() != "compra" && request.Operacao.ToLower() != "venda")
        {
            return BadRequest("Operação deve ser 'compra' ou 'venda'");
        }

        try
        {
            // Buscar investimento existente
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Investimento não encontrado");

            // Atualizar apenas os campos editáveis, preservando os demais
            existing.Tipo = request.Tipo.Trim();
            existing.Codigo = request.Codigo.Trim().ToUpper();
            existing.Valor = request.Valor;
            existing.Operacao = request.Operacao.Trim().ToLower();
            
            // Corrigir DateTimeKind para UTC (problema PostgreSQL)
            if (existing.CriadoEm.Kind != DateTimeKind.Utc)
            {
                existing.CriadoEm = DateTime.SpecifyKind(existing.CriadoEm, DateTimeKind.Utc);
            }
            // AlteradoEm será atualizado no repositório com UTC correto

            var updated = await _repository.UpdateAsync(existing);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Dados inválidos: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"Erro de operação: {ex.Message}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar investimento: {ex.Message}");
        }
    }

    /// <summary>
    /// 🗑️ Remove um investimento permanentemente
    /// </summary>
    /// <param name="id">ID do investimento a ser removido</param>
    /// <returns>Confirmação de remoção</returns>
    /// <response code="204">Investimento removido com sucesso</response>
    /// <response code="404">Investimento não encontrado</response>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "🗑️ Deletar investimento",
        Description = "⚠️ ATENÇÃO: Exclui permanentemente um investimento do sistema. Ação irreversível!"
    )]
    [SwaggerResponse(204, "Investimento removido com sucesso")]
    [SwaggerResponse(404, "Investimento não encontrado")]
    public async Task<ActionResult> Delete(
        [FromRoute, SwaggerParameter("ID do investimento a ser removido", Required = true)] Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _repository.DeleteAsync(id);
        return NoContent();
    }

    // Pesquisas com LINQ (10%)
    
    /// <summary>
    /// 📊 Filtra investimentos por tipo (LINQ)
    /// </summary>
    /// <param name="tipo">Tipo de investimento (ex: Ação, CDB, Tesouro)</param>
    /// <returns>Todos os investimentos do tipo escolhido</returns>
    /// <response code="200">Lista filtrada por tipo</response>
    [HttpGet("tipo/{tipo}")]
    [SwaggerOperation(
        Summary = "📊 Buscar por TIPO de investimento [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa Where() + OrderByDescending() para filtrar investimentos por tipo. Digite um tipo (Ação, CDB, Tesouro, etc.) para ver todos os investimentos desse tipo."
    )]
    [SwaggerResponse(200, "Investimentos filtrados por tipo", typeof(IEnumerable<Investimento>))]
    public async Task<ActionResult<IEnumerable<Investimento>>> GetByTipo(
        [FromRoute, SwaggerParameter("Tipo: Ação, CDB, Tesouro, etc.", Required = true)] string tipo)
    {
        var investimentos = await _repository.GetByTipoAsync(tipo);
        return Ok(investimentos);
    }

    /// <summary>
    /// 💰 Filtra por COMPRAS ou VENDAS (LINQ)
    /// </summary>
    /// <param name="operacao">Tipo de operação (compra ou venda)</param>
    /// <returns>Todas as compras OU todas as vendas</returns>
    /// <response code="200">Lista filtrada por operação</response>
    [HttpGet("operacao/{operacao}")]
    [SwaggerOperation(
        Summary = "💰 Ver só COMPRAS ou só VENDAS [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa Where() + OrderByDescending() para filtrar por operação. Digite 'compra' para ver todas as compras ou 'venda' para ver todas as vendas."
    )]
    [SwaggerResponse(200, "Investimentos filtrados por operação", typeof(IEnumerable<Investimento>))]
    public async Task<ActionResult<IEnumerable<Investimento>>> GetByOperacao(
        [FromRoute, SwaggerParameter("Digite: 'compra' ou 'venda'", Required = true)] string operacao)
    {
        var investimentos = await _repository.GetByOperacaoAsync(operacao);
        return Ok(investimentos);
    }

    /// <summary>
    /// 💵 Quanto uma pessoa tem investido - saldo líquido (LINQ)
    /// </summary>
    /// <param name="userCpf">CPF do usuário</param>
    /// <returns>Valor líquido: compras menos vendas</returns>
    /// <response code="200">Saldo calculado</response>
    [HttpGet("saldo/{userCpf}")]
    [SwaggerOperation(
        Summary = "💵 SALDO de uma pessoa [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa Join() + Where() + Select() + SumAsync() para calcular saldo líquido. COMPRAS (+) menos VENDAS (-) = saldo total investido."
    )]
    [SwaggerResponse(200, "Saldo líquido calculado", typeof(object))]
    public async Task<ActionResult<decimal>> GetTotalValueByUser(
        [FromRoute, SwaggerParameter("CPF apenas números", Required = true)] string userCpf)
    {
        var total = await _repository.GetTotalValueByUserAsync(userCpf);
        return Ok(new { UserCpf = userCpf, SaldoLiquido = total, Explicacao = "Compras (+) menos Vendas (-)" });
    }

    /// <summary>
    /// ⏰ Investimentos mais recentes (LINQ)
    /// </summary>
    /// <param name="days">Últimos quantos dias? (padrão: 30 dias)</param>
    /// <returns>Investimentos dos últimos N dias</returns>
    /// <response code="200">Investimentos recentes</response>
    [HttpGet("recentes")]
    [SwaggerOperation(
        Summary = "⏰ Ver investimentos RECENTES [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa Where() + OrderByDescending() para filtrar por data. Mostra investimentos dos últimos N dias (padrão: 30 dias)."
    )]
    [SwaggerResponse(200, "Lista de investimentos recentes", typeof(IEnumerable<Investimento>))]
    public async Task<ActionResult<IEnumerable<Investimento>>> GetRecentInvestments(
        [FromQuery, SwaggerParameter("Últimos quantos dias? (padrão: 30)")] int days = 30)
    {
        var investimentos = await _repository.GetRecentInvestmentsAsync(days);
        return Ok(investimentos);
    }

    /// <summary>
    /// 📈 Dashboard: estatísticas por tipo de investimento (LINQ)
    /// </summary>
    /// <returns>Resumo completo: quantos e quanto por tipo</returns>
    /// <response code="200">Dashboard resumo</response>
    [HttpGet("dashboard")]
    [SwaggerOperation(
        Summary = "📈 DASHBOARD - Resumo geral [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa GroupBy() + Select() + Count() + Sum() + Average() para estatísticas por tipo. Veja resumo completo de cada categoria de investimento."
    )]
    [SwaggerResponse(200, "Dashboard com estatísticas por tipo", typeof(IEnumerable<object>))]
    public async Task<ActionResult<IEnumerable<object>>> GetInvestmentSummaryByType()
    {
        var summary = await _repository.GetInvestmentSummaryByTypeAsync();
        return Ok(summary);
    }

    /// <summary>
    /// 👥 Lista de todos os CPFs que têm investimentos (LINQ)
    /// </summary>
    /// <returns>Todos os CPFs únicos da base</returns>
    /// <response code="200">Lista de CPFs</response>
    [HttpGet("usuarios")]
    [SwaggerOperation(
        Summary = "👥 Lista de PESSOAS que investem [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa Join() + Select() + Distinct() + OrderBy() para listar CPFs únicos. Mostra todos os CPFs que têm investimentos."
    )]
    [SwaggerResponse(200, "Lista de CPFs únicos", typeof(IEnumerable<string>))]
    public async Task<ActionResult<IEnumerable<string>>> GetAllUserCpfs()
    {
        var cpfs = await _repository.GetAllUserCpfsAsync();
        return Ok(cpfs);
    }
}