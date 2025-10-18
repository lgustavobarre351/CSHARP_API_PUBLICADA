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
// Railway específico - verificar DATABASE_URL primeiro
string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found");

Console.WriteLine($"🔗 Using connection: {(Environment.GetEnvironmentVariable("DATABASE_URL") != null ? "DATABASE_URL (Railway)" : "appsettings")}");

// Configurar Npgsql para usar comportamento legacy de timestamp (mais tolerante)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Configurações específicas para Railway/produção
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(30);
    });
    
    // Log detalhado em produção para debug
    if (builder.Environment.IsProduction())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information);
        options.EnableSensitiveDataLogging(false);
        options.EnableDetailedErrors(true); // Habilitar para debug
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

// --- Inicialização do Banco (Simplificado) ---
Console.WriteLine("�️ Configuração de banco concluída");

// --- Middleware Pipeline ---
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

// Redirecionar raiz para Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Endpoint simples de debug
app.MapGet("/debug", () => 
{
    return Results.Ok(new
    {
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
        Port = Environment.GetEnvironmentVariable("PORT"),
        HasDatabaseUrl = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")),
        Timestamp = DateTime.UtcNow
    });
});

app.MapControllers();

// --- Configuração Multi-Ambiente (Local + Cloud) ---
var environment = app.Environment.EnvironmentName;
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080"; // Railway/Render usam 8080 como padrão

if (environment == "Development")
{
    // Desenvolvimento local
    Console.WriteLine("🚀 API rodando em ambiente de DESENVOLVIMENTO");
    Console.WriteLine($"📋 Swagger Local: http://localhost:{port}/swagger");
    
    app.Run($"http://localhost:{port}");
}
else
{
    // Produção (Railway, Render, etc.) - usar 0.0.0.0 e porta dinâmica
    Console.WriteLine("🌍 API rodando em ambiente de PRODUÇÃO");
    Console.WriteLine($"🚀 Porta: {port}");
    Console.WriteLine($"📋 Swagger: /swagger");
    
    // Configurar URLs para produção
    var urls = $"http://0.0.0.0:{port}";
    app.Run(urls);
}