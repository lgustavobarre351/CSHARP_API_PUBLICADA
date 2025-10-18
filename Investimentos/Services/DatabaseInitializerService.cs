using Microsoft.EntityFrameworkCore;
using ProjetoInvestimentos.Data;
using ProjetoInvestimentos.Models;

namespace ProjetoInvestimentos.Services;

/// <summary>
/// Serviço para inicialização e configuração do banco de dados
/// </summary>
public interface IDatabaseInitializerService
{
    Task InitializeAsync();
}

public class DatabaseInitializerService : IDatabaseInitializerService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseInitializerService> _logger;

    public DatabaseInitializerService(AppDbContext context, ILogger<DatabaseInitializerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando verificação/criação das tabelas do banco de dados...");

            // Garantir que o banco existe
            await _context.Database.EnsureCreatedAsync();

            // Verificar se as tabelas existem e criar se necessário
            await CreateTablesIfNotExistAsync();

            // Criar usuário de exemplo se não existir
            await CreateSampleUserIfNotExistAsync();

            _logger.LogInformation("Banco de dados inicializado com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar banco de dados: {Message}", ex.Message);
            throw;
        }
    }

    private async Task CreateTablesIfNotExistAsync()
    {
        try
        {
            // Primeiro, verificar se as tabelas existem e criar com estrutura básica
            var createUserProfilesTable = @"
                CREATE TABLE IF NOT EXISTS public.user_profiles (
                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    cpf VARCHAR(11) NOT NULL UNIQUE,
                    nome VARCHAR(200)
                );";

            var createInvestimentosTable = @"
                CREATE TABLE IF NOT EXISTS public.investimentos (
                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    user_id UUID REFERENCES public.user_profiles(id),
                    tipo VARCHAR(50) NOT NULL,
                    codigo VARCHAR(20) NOT NULL,
                    valor DECIMAL(18,2) NOT NULL,
                    operacao VARCHAR(20) NOT NULL,
                    criado_em TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
                    alterado_em TIMESTAMP WITH TIME ZONE DEFAULT NOW()
                );";

            // Tentar adicionar a coluna created_at se não existir
            var addCreatedAtColumn = @"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                                  WHERE table_name='user_profiles' AND column_name='created_at') THEN
                        ALTER TABLE public.user_profiles 
                        ADD COLUMN created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW();
                    END IF;
                END $$;";

            var createIndexes = @"
                CREATE INDEX IF NOT EXISTS idx_user_profiles_cpf ON public.user_profiles(cpf);
                CREATE INDEX IF NOT EXISTS idx_investimentos_user_id ON public.investimentos(user_id);
                CREATE INDEX IF NOT EXISTS idx_investimentos_tipo ON public.investimentos(tipo);
                CREATE INDEX IF NOT EXISTS idx_investimentos_criado_em ON public.investimentos(criado_em);";

            await _context.Database.ExecuteSqlRawAsync(createUserProfilesTable);
            await _context.Database.ExecuteSqlRawAsync(createInvestimentosTable);
            await _context.Database.ExecuteSqlRawAsync(addCreatedAtColumn);
            await _context.Database.ExecuteSqlRawAsync(createIndexes);

            _logger.LogInformation("Tabelas verificadas/criadas com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao criar tabelas (podem já existir): {Message}", ex.Message);
        }
    }

    private async Task CreateSampleUserIfNotExistAsync()
    {
        try
        {
            // Verificar se já existe usuário de exemplo
            var existingUser = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.Cpf == "52604928238");

            if (existingUser == null)
            {
                var sampleUser = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    Cpf = "52604928238",
                    Nome = "Usuário de Teste"
                    // CreatedAt será definido pelo banco (DEFAULT NOW())
                };

                _context.UserProfiles.Add(sampleUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário de exemplo criado: CPF 52604928238");
            }
            else
            {
                _logger.LogInformation("Usuário de exemplo já existe: CPF 52604928238");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao criar usuário de exemplo: {Message}", ex.Message);
        }
    }
}