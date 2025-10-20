# 💰 API de Investimentos - Challenge FIAP 2025

> **API RESTful para gerenciamento de investimentos com ASP.NET Core 9.0 e PostgreSQL**

🌐 **API em Produção:** https://sua-api-investimentos-production.up.railway.app/swagger

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791.svg)](https://www.postgresql.org/)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D.svg)](https://swagger.io/)
[![Railway](https://img.shields.io/badge/Deploy-Railway-success.svg)](https://sua-api-investimentos-production.up.railway.app)

## 👥 **EQUIPE**
- **André Lambert** - RM: 99148 | **Felipe Cortez** - RM: 99750 | **Julia Lins** - RM: 98690 | **Luis Barreto** - RM: 99210 | **Victor Aranda** - RM: 99667

## 🚀 **COMO RODAR (2 MINUTOS)**

**📋 Pré-requisito:** .NET 9.0 SDK ([baixar aqui](https://dotnet.microsoft.com/download/dotnet/9.0))

### **💻 Opção 1: Terminal/CMD**
```bash
# 1. Clone o repositório
git clone https://github.com/lgustavobarre351/CSHARP_API_PUBLICADA.git

# 2. Entre na pasta e execute
cd CSHARP_API_PUBLICADA/Investimentos
dotnet run

# 3. Abra no navegador
# http://localhost:8080/swagger
```

### **🔧 Opção 2: VS Code**
```bash
# 1. Clone e abra no VS Code
git clone https://github.com/lgustavobarre351/CSHARP_API_PUBLICADA.git
code CSHARP_API_PUBLICADA

# 2. Abra terminal integrado (Ctrl + `) e execute:
cd Investimentos
dotnet run

# OU use F5 para Debug (vai abrir automaticamente no navegador)
```

### **⚡ Opção 3: Visual Studio**
```bash
# 1. Clone e abra o .sln
git clone https://github.com/lgustavobarre351/CSHARP_API_PUBLICADA.git
# Abrir: CSharp.sln no Visual Studio

# 2. Pressione F5 ou clique em "Play" ▶️
```

**✅ Zero configuração necessária! Banco já configurado na nuvem.**

> **⚡ Verificar .NET:** `dotnet --version` (deve mostrar 9.x.x)

## 📋 **O QUE FAZ**

**Sistema de investimentos com:**
- 💰 **CRUD completo** de investimentos e usuários
- 🔍 **6 consultas LINQ** (filtros, agregações, joins)
- 🌐 **APIs externas** (cotações B3, HG Brasil) 
- ☁️ **Deploy na nuvem** (PostgreSQL Supabase)
- 📖 **Documentação Swagger** completa

## 🏗️ **ARQUITETURA DO SISTEMA**

![Diagrama de Arquitetura](Diagrama.png)

**Fluxo:** Cliente → Controllers → Services/Repository → Entity Framework → PostgreSQL + APIs Externas

## 🎯 **ENDPOINTS PRINCIPAIS**

```http
# 📊 Investimentos
GET    /api/investimentos              # Listar todos
POST   /api/investimentos              # Criar novo
GET    /api/investimentos/saldo/{cpf}  # Saldo por usuário [LINQ]
GET    /api/investimentos/dashboard    # Dashboard estatístico [LINQ]

# 👥 Usuários  
GET    /api/usuarios                   # Listar usuários
POST   /api/usuarios                   # Criar usuário

# 🌐 APIs Externas
GET    /api/apisexternas/cotacao/{codigo}        # Cotação em tempo real
GET    /api/apisexternas/codigos-b3              # Códigos B3 válidos
```

## ✅ **CRITÉRIOS ATENDIDOS (100%)**

| Critério | Peso | Status | Localização |
|----------|------|--------|-------------|
| **ASP.NET Core + EF + CRUD** | 35% | ✅ | `Controllers/` + `Repositories/` |
| **Consultas LINQ** | 10% | ✅ | `EfInvestimentoRepository.cs` (6 consultas) |
| **Deploy Cloud** | 15% | ✅ | Railway (online) + PostgreSQL Supabase |
| **APIs Externas** | 20% | ✅ | Brapi + HG Brasil (`ApisExternasController`) |
| **Documentação** | 10% | ✅ | Swagger + README |
| **Diagrama Arquitetura** | 10% | ✅ | `Diagrama.png` (acima) |

## 🔍 **CONSULTAS LINQ IMPLEMENTADAS**

```csharp
// 1. Filtro por tipo - Where + OrderBy
.Where(i => i.Tipo.ToLower() == tipo.ToLower())
.OrderByDescending(i => i.CriadoEm)

// 2. Saldo líquido - Join + Sum  
from i in _context.Investimentos
join u in _context.UserProfiles on i.UserId equals u.Id
where u.Cpf == userCpf
select i.Operacao.ToLower() == "compra" ? i.Valor : -i.Valor
).SumAsync()

// 3. Dashboard - GroupBy + Count + Sum + Average
.GroupBy(i => i.Tipo)
.Select(g => new {
    Tipo = g.Key,
    Quantidade = g.Count(),
    ValorTotal = g.Sum(i => i.Valor),
    ValorMedio = g.Average(i => i.Valor)
})
```

## 🌐 **TECNOLOGIAS**

- **Backend**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core
- **Banco**: PostgreSQL (Supabase)
- **APIs**: Brapi, HG Brasil  
- **Docs**: Swagger/OpenAPI
- **Deploy**: Railway, Render, Fly.io

## 📖 **ESTRUTURA DO PROJETO**

```
Investimentos/
├── Controllers/          # 🎮 API endpoints
├── Models/              # 📊 Entidades (Investimento, User)  
├── Repositories/        # 📚 Acesso a dados + LINQ
├── Services/            # ⚙️ Lógica de negócio
├── Data/                # 🗄️ Entity Framework context
└── Swagger/             # 📖 Documentação
```

---

**🎯 Local:** `http://localhost:8080/swagger` **após executar `dotnet run`**
**🌐 Online:** https://sua-api-investimentos-production.up.railway.app/swagger

## 🧪 **COMO TESTAR**

### **🔥 Testar Online (SEM instalar nada):**
1. **Acesse:** https://sua-api-investimentos-production.up.railway.app/swagger
2. **Use os exemplos abaixo** diretamente no Swagger online
3. **Dados já carregados** - pode testar imediatamente!

### **💻 Testar Local:**
Execute `dotnet run` e acesse: `http://localhost:8080/swagger`

### **1. Criar um investimento**
```json
POST /api/investimentos
{
  "userCpf": "12345678901",
  "tipo": "Ação",
  "codigo": "PETR4", 
  "valor": 1500.50,
  "operacao": "compra"
}
```

### **2. Consultar saldo**
```
GET /api/investimentos/saldo/12345678901
```

### **3. Ver cotação em tempo real**
```
GET /api/apisexternas/cotacao/PETR4
```

### **4. Dashboard estatístico**
```
GET /api/investimentos/dashboard
```

## 🆘 **PROBLEMAS COMUNS**

- **Erro de porta:** Se 8080 estiver ocupada, a API usará outra porta automática
- **Erro .NET:** Instale .NET 9.0 SDK 
- **Erro de conexão:** Banco já configurado na nuvem, não precisa PostgreSQL local
- **Swagger não abre:** Aguarde mensagem "Now listening on..." no terminal

