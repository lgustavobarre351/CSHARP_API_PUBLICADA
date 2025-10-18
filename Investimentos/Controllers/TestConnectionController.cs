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
            Summary = "Testa conexão com banco",
            Description = "Verifica se a conexão com o PostgreSQL está funcionando"
        )]
        [SwaggerResponse(200, "Conexão OK", typeof(object))]
        [SwaggerResponse(400, "Erro de conexão", typeof(object))]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                // Log da connection string (sem senha para segurança)
                var safeConnectionString = connectionString?.Replace("ju153074", "***");
                Console.WriteLine($"Testing connection: {safeConnectionString}");

                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                
                // Teste simples de query
                using var command = new NpgsqlCommand("SELECT version();", connection);
                var result = await command.ExecuteScalarAsync();
                
                return Ok(new { 
                    status = "Success", 
                    message = "Conexão estabelecida com sucesso!",
                    postgresVersion = result?.ToString(),
                    connectionString = safeConnectionString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    status = "Error", 
                    message = ex.Message,
                    type = ex.GetType().Name,
                    stackTrace = ex.StackTrace
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