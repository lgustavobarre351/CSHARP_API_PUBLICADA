using Microsoft.EntityFrameworkCore;
using ProjetoInvestimentos.Data;
using ProjetoInvestimentos.Repositories;
using ProjetoInvestimentos.Services;
using ProjetoInvestimentos.Swagger;

var builder = WebApplication.CreateBuilder(args);

// --- Logging Configuration para Railway ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

Console.WriteLine("🚀 INICIANDO APLICAÇÃO...");
Console.WriteLine($"   Args: {string.Join(", ", args)}");

// --- Services Configuration ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Investimentos API",
        Version = "v1",
        Description = "API completa para gerenciamento de investimentos com CRUD, consultas LINQ e integração com APIs externas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipe Challenge XP",
            Email = "contato@challengexp.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar exemplos
    c.EnableAnnotations();
    c.SchemaFilter<SwaggerExamplesFilter>();
    
    // Configurar tags
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
});

// --- Entity Framework ---
// Railway pode usar DATABASE_URL ao invés de DefaultConnection
string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found");

// Configurar Npgsql para usar comportamento legacy de timestamp (mais tolerante)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    try
    {
        Console.WriteLine("🗄️ Configurando Entity Framework...");
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3, 
                maxRetryDelay: TimeSpan.FromSeconds(10), 
                errorCodesToAdd: null
            );
            npgsqlOptions.CommandTimeout(60); // Aumentar timeout
        });
        
        // Configurações para Railway
        if (builder.Environment.IsProduction())
        {
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        }
        else
        {
            options.EnableSensitiveDataLogging(true);
            options.EnableDetailedErrors(true);
        }
        
        Console.WriteLine("✅ Entity Framework configurado");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao configurar EF: {ex.Message}");
        throw;
    }
});

// --- Repository ---
builder.Services.AddScoped<IInvestimentoRepository, EfInvestimentoRepository>();

// --- Serviços ---
builder.Services.AddSingleton<IB3ValidationService, B3ValidationService>();

// --- HttpClient para APIs externas ---
builder.Services.AddHttpClient();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- Debug Info para Railway ---
Console.WriteLine("🔍 RAILWAY DEBUG INFO:");
Console.WriteLine($"   ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
Console.WriteLine($"   PORT: {Environment.GetEnvironmentVariable("PORT")}");
Console.WriteLine($"   DATABASE_URL exists: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL"))}");
Console.WriteLine($"   RAILWAY_STATIC_URL: {Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL")}");
Console.WriteLine($"   Connection String Source: {(Environment.GetEnvironmentVariable("DATABASE_URL") != null ? "DATABASE_URL" : "appsettings")}");

// --- Inicialização do Banco (Assíncrona) ---
_ = Task.Run(async () =>
{
    try
    {
        await Task.Delay(2000); // Aguardar 2s para app inicializar
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Console.WriteLine("🔄 Testando conexão com banco de dados...");
            
            var canConnect = await context.Database.CanConnectAsync();
            if (canConnect)
            {
                Console.WriteLine("✅ Conexão com banco de dados estabelecida com sucesso");
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("✅ Tabelas verificadas/criadas com sucesso");
            }
            else
            {
                Console.WriteLine("❌ Não foi possível conectar ao banco de dados");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao conectar com banco de dados:");
        Console.WriteLine($"   Tipo: {ex.GetType().Name}");
        Console.WriteLine($"   Mensagem: {ex.Message}");
    }
});

// --- Middleware Pipeline ---
Console.WriteLine("🔧 Configurando middleware...");

// Middleware de tratamento de exceções personalizado
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ EXCEÇÃO NÃO TRATADA: {ex.Message}");
        Console.WriteLine($"   Stack: {ex.StackTrace}");
        
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = "Internal Server Error",
            message = ex.Message,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path
        };
        
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

// Middleware de tratamento de exceções padrão
app.UseExceptionHandler("/error");

// Swagger habilitado também em produção para demonstração
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Investimentos API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Investimentos API - Documentação";
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    c.DefaultModelsExpandDepth(-1);
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.EnableValidator();
});

app.UseCors();
app.UseRouting();

// --- Endpoints de Sistema ---
// Endpoint básico de teste
app.MapGet("/ping", () =>
{
    Console.WriteLine("🏓 Ping recebido!");
    return Results.Ok(new { 
        message = "pong", 
        timestamp = DateTime.UtcNow,
        status = "API está funcionando!"
    });
});

// Redirecionar raiz para Swagger
app.MapGet("/", () => 
{
    Console.WriteLine("🏠 Root endpoint chamado - redirecionando para /swagger");
    return Results.Redirect("/swagger");
});

// Endpoint de tratamento de erros
app.MapGet("/error", () => 
{
    return Results.Problem(
        title: "Erro interno do servidor",
        detail: "Ocorreu um erro inesperado. Verifique os logs para mais detalhes.",
        statusCode: 500
    );
});

// Health check simples
app.MapGet("/health", () =>
{
    Console.WriteLine("🩺 Health check chamado");
    return Results.Ok(new { 
        status = "Healthy", 
        timestamp = DateTime.UtcNow,
        version = "1.0.0",
        environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
    });
});

// Health check do banco separado
app.MapGet("/health/database", async (AppDbContext context) =>
{
    try
    {
        Console.WriteLine("🗄️ Database health check chamado");
        var canConnect = await context.Database.CanConnectAsync();
        return Results.Ok(new { 
            status = canConnect ? "Healthy" : "Unhealthy", 
            database = canConnect ? "Connected" : "Disconnected", 
            timestamp = DateTime.UtcNow 
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database health check error: {ex.Message}");
        return Results.Ok(new { 
            status = "Unhealthy", 
            error = ex.Message, 
            timestamp = DateTime.UtcNow 
        });
    }
});

app.MapControllers();

// --- Configuração Multi-Ambiente (Local + Cloud) ---
var environment = app.Environment.EnvironmentName;

// Railway específico - usar variável RAILWAY_STATIC_URL se disponível
var railwayPort = Environment.GetEnvironmentVariable("PORT");
var port = railwayPort ?? "8080";

Console.WriteLine("═══════════════════════════════════════");
Console.WriteLine("🚀 INICIANDO SERVIDOR WEB");
Console.WriteLine("═══════════════════════════════════════");
Console.WriteLine($"🌍 Environment: {environment}");
Console.WriteLine($"🚀 Port: {port}");
Console.WriteLine($"📋 Railway URL: {Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL")}");
Console.WriteLine($"🔗 Bind URL: http://0.0.0.0:{port}");
Console.WriteLine("═══════════════════════════════════════");

if (environment == "Development")
{
    Console.WriteLine("🚀 API rodando em ambiente de DESENVOLVIMENTO");
    Console.WriteLine($"📋 Swagger Local: http://localhost:{port}/swagger");
    Console.WriteLine($"🩺 Health: http://localhost:{port}/health");
    
    app.Run($"http://localhost:{port}");
}
else
{
    Console.WriteLine("🌍 API rodando em ambiente de PRODUÇÃO (Railway)");
    Console.WriteLine($"🚀 Binding to: 0.0.0.0:{port}");
    Console.WriteLine($"📋 Swagger: /swagger");
    Console.WriteLine($"🩺 Health: /health");
    Console.WriteLine("🔄 Iniciando servidor...");
    
    // Railway específico - bind em todas as interfaces
    try
    {
        app.Run($"http://0.0.0.0:{port}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"💥 ERRO CRÍTICO AO INICIAR SERVIDOR: {ex.Message}");
        Console.WriteLine($"Stack: {ex.StackTrace}");
        throw;
    }
}