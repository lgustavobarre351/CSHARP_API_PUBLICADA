using Microsoft.EntityFrameworkCore;
using ProjetoInvestimentos.Data;
using ProjetoInvestimentos.Repositories;
using ProjetoInvestimentos.Services;
using ProjetoInvestimentos.Swagger;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(30);
    });
    
    // Configurações adicionais para produção
    if (!builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(false);
        options.EnableDetailedErrors(false);
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

// --- Inicialização do Banco ---
try
{
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
    Console.WriteLine($"   Stack: {ex.StackTrace}");
    // Continue sem falhar - para debug em produção
}

// --- Middleware Pipeline ---
// Middleware de tratamento de exceções
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
// Redirecionar raiz para Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Endpoint de tratamento de erros
app.MapGet("/error", () => 
{
    return Results.Problem(
        title: "Erro interno do servidor",
        detail: "Ocorreu um erro inesperado. Verifique os logs para mais detalhes.",
        statusCode: 500
    );
});

// Health check
app.MapGet("/health", async (AppDbContext context) =>
{
    try
    {
        await context.Database.CanConnectAsync();
        return Results.Ok(new { status = "Healthy", database = "Connected", timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Database connection failed",
            detail: ex.Message,
            statusCode: 503
        );
    }
});

app.MapControllers();

// --- Configuração Multi-Ambiente (Local + Cloud) ---
var environment = app.Environment.EnvironmentName;

// Railway específico - usar variável RAILWAY_STATIC_URL se disponível
var railwayPort = Environment.GetEnvironmentVariable("PORT");
var port = railwayPort ?? "8080";

Console.WriteLine($"🌍 Environment: {environment}");
Console.WriteLine($"🚀 Port: {port}");
Console.WriteLine($"📋 Railway URL: {Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL")}");

if (environment == "Development")
{
    // Desenvolvimento local
    Console.WriteLine("🚀 API rodando em ambiente de DESENVOLVIMENTO");
    Console.WriteLine($"📋 Swagger Local: http://localhost:{port}/swagger");
    
    app.Run($"http://localhost:{port}");
}
else
{
    // Produção (Railway, Render, etc.)
    Console.WriteLine("🌍 API rodando em ambiente de PRODUÇÃO (Railway)");
    Console.WriteLine($"🚀 Binding to: 0.0.0.0:{port}");
    Console.WriteLine($"📋 Swagger: /swagger");
    
    // Railway específico - bind em todas as interfaces
    app.Run($"http://0.0.0.0:{port}");
}