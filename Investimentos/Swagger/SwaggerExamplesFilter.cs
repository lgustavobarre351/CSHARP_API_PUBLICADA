using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ProjetoInvestimentos.Models;
using ProjetoInvestimentos.Controllers;

namespace ProjetoInvestimentos.Swagger;

/// <summary>
/// Provedor de exemplos para documentação Swagger
/// </summary>
public class SwaggerExamplesFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(Investimento))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["id"] = new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                ["userCpf"] = new Microsoft.OpenApi.Any.OpenApiString("12345678901"),
                ["userId"] = new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                ["tipo"] = new Microsoft.OpenApi.Any.OpenApiString("Ação"),
                ["codigo"] = new Microsoft.OpenApi.Any.OpenApiString("PETR4"),
                ["valor"] = new Microsoft.OpenApi.Any.OpenApiDouble(1500.75),
                ["operacao"] = new Microsoft.OpenApi.Any.OpenApiString("compra"),
                ["criadoEm"] = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                ["alteradoEm"] = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"))
            };
        }
        
        if (context.Type == typeof(UserProfile))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["id"] = new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                ["cpf"] = new Microsoft.OpenApi.Any.OpenApiString("12345678901"),
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("joao.silva@email.com"),
                ["nome"] = new Microsoft.OpenApi.Any.OpenApiString("João Silva"),
                ["dados"] = new Microsoft.OpenApi.Any.OpenApiString("{\"nome\":\"João Silva\"}"),
                ["criadoEm"] = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                ["alteradoEm"] = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"))
            };
        }
        
        if (context.Type == typeof(CreateUserRequest))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["cpf"] = new Microsoft.OpenApi.Any.OpenApiString("12345678901"),
                ["nome"] = new Microsoft.OpenApi.Any.OpenApiString("João Silva"),
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("joao.silva@email.com")
            };
        }
        
        if (context.Type == typeof(UpdateUserRequest))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["nome"] = new Microsoft.OpenApi.Any.OpenApiString("João Silva Santos")
            };
        }
    }
}