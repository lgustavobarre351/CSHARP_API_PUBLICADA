using Microsoft.EntityFrameworkCore;
using ProjetoInvestimentos.Data;
using ProjetoInvestimentos.Models;

namespace ProjetoInvestimentos.Repositories;

public interface IInvestimentoRepository
{
    Task<IEnumerable<Investimento>> GetAllAsync();
    Task<Investimento?> GetByIdAsync(Guid id);
    Task<IEnumerable<Investimento>> GetByUserCpfAsync(string userCpf);
    Task<Investimento> CreateAsync(Investimento investimento);
    Task<Investimento> UpdateAsync(Investimento investimento);
    Task DeleteAsync(Guid id);
    
    // LINQ queries (peso 10%)
    Task<IEnumerable<Investimento>> GetByTipoAsync(string tipo);
    Task<IEnumerable<Investimento>> GetByOperacaoAsync(string operacao);
    Task<decimal> GetTotalValueByUserAsync(string userCpf);
    Task<IEnumerable<Investimento>> GetRecentInvestmentsAsync(int days = 30);
    Task<IEnumerable<object>> GetInvestmentSummaryByTypeAsync();
    Task<IEnumerable<string>> GetAllUserCpfsAsync();
}

public class EfInvestimentoRepository : IInvestimentoRepository
{
    private readonly AppDbContext _context;

    public EfInvestimentoRepository(AppDbContext context)
    {
        _context = context;
    }

    // Método utilitário para garantir que DateTime seja UTC
    private static DateTime EnsureUtc(DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc ? 
               dateTime : 
               DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    public async Task<IEnumerable<Investimento>> GetAllAsync()
    {
        // LINQ: Consulta para obter todos os investimentos ordenados por data de criação
        return await _context.Investimentos
            .OrderByDescending(i => i.CriadoEm)
            .ToListAsync();
    }

    public async Task<Investimento?> GetByIdAsync(Guid id)
    {
        // LINQ: Busca investimento específico por ID
        return await _context.Investimentos
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Investimento>> GetByUserCpfAsync(string userCpf)
    {
        // LINQ: Filtra investimentos por CPF do usuário e ordena por data
        return await _context.Investimentos
            .Where(i => i.UserCpf == userCpf)
            .OrderByDescending(i => i.CriadoEm)
            .ToListAsync();
    }

    public async Task<Investimento> CreateAsync(Investimento investimento)
    {
        try
        {
            investimento.Id = Guid.NewGuid();
            investimento.CriadoEm = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            investimento.AlteradoEm = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            // Validar se CPF foi fornecido
            if (string.IsNullOrEmpty(investimento.UserCpf))
            {
                throw new ArgumentException("CPF do usuário é obrigatório");
            }

            // LINQ: Buscar ou criar o usuário
            var user = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.Cpf == investimento.UserCpf);
            
            if (user == null)
            {
                // Criar usuário automaticamente se não existir
                user = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    Cpf = investimento.UserCpf,
                    Email = null, // Email pode ser definido posteriormente
                    Dados = null, // Dados podem ser definidos posteriormente
                    CriadoEm = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    AlteradoEm = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };
                
                _context.UserProfiles.Add(user);
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    var innerMessage = dbEx.InnerException?.Message ?? "Sem detalhes adicionais";
                    throw new InvalidOperationException($"Erro de banco ao criar usuário com CPF {investimento.UserCpf}: {dbEx.Message}. Inner: {innerMessage}", dbEx);
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? "Sem detalhes adicionais";
                    throw new InvalidOperationException($"Erro ao criar usuário com CPF {investimento.UserCpf}: {ex.Message}. Inner: {innerMessage}", ex);
                }
            }
            
            investimento.UserId = user.Id;
            // Manter UserCpf - agora é mapeado para coluna user_cpf no banco

            // Adicionar investimento
            _context.Investimentos.Add(investimento);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? "Sem detalhes adicionais";
                throw new InvalidOperationException($"Erro de banco ao salvar investimento: {dbEx.Message}. Inner: {innerMessage}", dbEx);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "Sem detalhes adicionais";
                throw new InvalidOperationException($"Erro ao salvar investimento: {ex.Message}. Inner: {innerMessage}", ex);
            }
            
            return investimento;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao processar investimento: {ex.Message}", ex);
        }
    }

    public async Task<Investimento> UpdateAsync(Investimento investimento)
    {
        try
        {
            // LINQ: Desanexar qualquer entidade que possa estar sendo rastreada
            var tracked = _context.ChangeTracker.Entries<Investimento>()
                .FirstOrDefault(e => e.Entity.Id == investimento.Id);
            if (tracked != null)
            {
                _context.Entry(tracked.Entity).State = EntityState.Detached;
            }

            // Criar um novo objeto garantindo que todos os DateTime sejam UTC
            var investimentoParaAtualizar = new Investimento
            {
                Id = investimento.Id,
                UserCpf = investimento.UserCpf,
                UserId = investimento.UserId,
                Tipo = investimento.Tipo,
                Codigo = investimento.Codigo,
                Valor = investimento.Valor,
                Operacao = investimento.Operacao,
                CriadoEm = EnsureUtc(investimento.CriadoEm),
                AlteradoEm = EnsureUtc(DateTime.UtcNow)
            };
            
            // Anexar e marcar como modificado
            _context.Entry(investimentoParaAtualizar).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            return investimentoParaAtualizar;
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "Sem detalhes adicionais";
            throw new InvalidOperationException($"Erro ao atualizar investimento: {ex.Message}. Inner: {innerMessage}", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var investimento = await GetByIdAsync(id);
        if (investimento != null)
        {
            _context.Investimentos.Remove(investimento);
            await _context.SaveChangesAsync();
        }
    }

    // LINQ Queries (peso 10%)
    public async Task<IEnumerable<Investimento>> GetByTipoAsync(string tipo)
    {
        // LINQ: Filtro por tipo de investimento com busca case-insensitive
        return await _context.Investimentos
            .Where(i => i.Tipo.ToLower() == tipo.ToLower())
            .OrderByDescending(i => i.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<Investimento>> GetByOperacaoAsync(string operacao)
    {
        // LINQ: Filtro por tipo de operação (compra/venda) com busca case-insensitive
        return await _context.Investimentos
            .Where(i => i.Operacao.ToLower() == operacao.ToLower())
            .OrderByDescending(i => i.CriadoEm)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalValueByUserAsync(string userCpf)
    {
        // LINQ: Join entre tabelas + cálculo de saldo (compras positivas, vendas negativas)
        return await (from i in _context.Investimentos
                      join u in _context.UserProfiles on i.UserId equals u.Id
                      where u.Cpf == userCpf
                      select i.Operacao.ToLower() == "compra" ? i.Valor : -i.Valor)
                     .SumAsync();
    }

    public async Task<IEnumerable<Investimento>> GetRecentInvestmentsAsync(int days = 30)
    {
        var cutoffDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-days), DateTimeKind.Utc);
        // LINQ: Filtro por data dos últimos N dias
        return await _context.Investimentos
            .Where(i => i.CriadoEm >= cutoffDate)
            .OrderByDescending(i => i.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<object>> GetInvestmentSummaryByTypeAsync()
    {
        // LINQ: GroupBy + agregações (Count, Sum, Average) para relatório por tipo
        return await _context.Investimentos
            .GroupBy(i => i.Tipo)
            .Select(g => new
            {
                Tipo = g.Key,
                TotalInvestimentos = g.Count(),
                ValorTotal = g.Sum(i => i.Operacao.ToLower() == "compra" ? i.Valor : -i.Valor),
                ValorMedio = g.Average(i => i.Valor)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllUserCpfsAsync()
    {
        // LINQ: Join + Distinct + OrderBy para listar CPFs únicos
        return await (from i in _context.Investimentos
                      join u in _context.UserProfiles on i.UserId equals u.Id
                      select u.Cpf)
                     .Distinct()
                     .OrderBy(cpf => cpf)
                     .ToListAsync();
    }
}