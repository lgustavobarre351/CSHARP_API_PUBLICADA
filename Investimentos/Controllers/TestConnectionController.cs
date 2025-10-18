using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjetoInvestimentos.Controllers
{
    /// <summary>
    /// Teste de conexão
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("4️⃣ UTILITÁRIOS - Testes de conexão e diagnósticos")]
    public class TestConnectionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TestConnectionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("test-connection")]
        [SwaggerOperation(
            Summary = "🔧 Testa conexão com banco Supabase",
            Description = "Verifica se a conexão com o PostgreSQL Supabase está funcionando e testa tabelas"
        )]
        [SwaggerResponse(200, "Conexão OK", typeof(object))]
        [SwaggerResponse(500, "Erro de conexão", typeof(object))]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                // Log da connection string (sem senha para segurança)
                var safeConnectionString = connectionString?.Replace("ju153074", "***");
                Console.WriteLine($"🔍 Testing connection: {safeConnectionString}");

                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine("✅ Connection opened successfully");
                
                // Teste de versão do PostgreSQL
                using var versionCommand = new NpgsqlCommand("SELECT version();", connection);
                var version = await versionCommand.ExecuteScalarAsync();
                
                // Teste se as tabelas existem
                var tablesQuery = @"
                    SELECT table_name 
                    FROM information_schema.tables 
                    WHERE table_schema = 'public' 
                    AND table_name IN ('user_profiles', 'investimentos');";
                    
                using var tablesCommand = new NpgsqlCommand(tablesQuery, connection);
                using var reader = await tablesCommand.ExecuteReaderAsync();
                
                var existingTables = new List<string>();
                while (await reader.ReadAsync())
                {
                    existingTables.Add(reader.GetString(0));
                }
                reader.Close();
                
                // Contar registros se as tabelas existirem
                int userCount = 0, investmentCount = 0;
                
                if (existingTables.Contains("user_profiles"))
                {
                    using var userCountCommand = new NpgsqlCommand("SELECT COUNT(*) FROM public.user_profiles;", connection);
                    userCount = Convert.ToInt32(await userCountCommand.ExecuteScalarAsync());
                }
                
                if (existingTables.Contains("investimentos"))
                {
                    using var investmentCountCommand = new NpgsqlCommand("SELECT COUNT(*) FROM public.investimentos;", connection);
                    investmentCount = Convert.ToInt32(await investmentCountCommand.ExecuteScalarAsync());
                }
                
                return Ok(new { 
                    status = "✅ Success", 
                    message = "Conexão estabelecida com sucesso!",
                    database = new
                    {
                        version = version?.ToString(),
                        host = "aws-1-us-east-1.pooler.supabase.com",
                        provider = "Supabase PostgreSQL"
                    },
                    tables = new
                    {
                        existing = existingTables,
                        user_profiles_count = userCount,
                        investimentos_count = investmentCount
                    },
                    timestamp = DateTime.UtcNow,
                    connectionString = safeConnectionString
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection error: {ex.Message}");
                return StatusCode(500, new { 
                    status = "❌ Error", 
                    message = ex.Message,
                    type = ex.GetType().Name,
                    timestamp = DateTime.UtcNow,
                    suggestions = new[]
                    {
                        "Verifique se o Supabase está online",
                        "Confirme as credenciais na connection string",
                        "Verifique se as tabelas foram criadas",
                        "Teste a conectividade de rede"
                    }
                });
            }
        }

        [HttpGet("test-tables")]
        public async Task<IActionResult> TestTables()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                
                // Verificar se as tabelas existem
                using var command = new NpgsqlCommand(@"
                    SELECT table_name 
                    FROM information_schema.tables 
                    WHERE table_schema = 'public' 
                    AND table_name IN ('user_profiles', 'investimentos')
                    ORDER BY table_name;", connection);
                
                var tables = new List<string>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tables.Add(reader.GetString(0));
                }
                
                return Ok(new { 
                    status = "Success", 
                    message = "Tabelas verificadas com sucesso!",
                    existingTables = tables
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    status = "Error", 
                    message = ex.Message,
                    type = ex.GetType().Name
                });
            }
        }
        [HttpGet("test-different-formats")]
        public async Task<IActionResult> TestDifferentFormats()
        {
            var results = new List<object>();
            
            // Diferentes formatos de connection string para testar
            var connectionStrings = new[]
            {
                "Host=aws-0-us-east-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.meawpenzwaxszxhweehh;Password=ju153074;Ssl Mode=Require;",
                "Host=aws-0-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.meawpenzwaxszxhweehh;Password=ju153074;Ssl Mode=Require;",
                "Host=aws-0-us-east-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres;Password=ju153074;Ssl Mode=Require;",
                "Host=aws-0-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres;Password=ju153074;Ssl Mode=Require;"
            };

            for (int i = 0; i < connectionStrings.Length; i++)
            {
                try
                {
                    using var connection = new NpgsqlConnection(connectionStrings[i]);
                    await connection.OpenAsync();
                    
                    results.Add(new { 
                        test = i + 1,
                        status = "Success", 
                        connectionString = connectionStrings[i].Replace("ju153074", "***")
                    });
                    
                    connection.Close();
                }
                catch (Exception ex)
                {
                    results.Add(new { 
                        test = i + 1,
                        status = "Error", 
                        message = ex.Message,
                        connectionString = connectionStrings[i].Replace("ju153074", "***")
                    });
                }
            }
            
            return Ok(results);
        }
    }
}