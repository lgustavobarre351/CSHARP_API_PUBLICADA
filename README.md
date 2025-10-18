# 💰 API de Investimentos - Challenge FIAP 2024

> **Sistema completo de gerenciamento de investimentos com ASP.NET Core 9.0 e PostgreSQL**

## 👥 **EQUIPE DESENVOLVEDORA**
- **André Lambert** - RM: 99148
- **Felipe Cortez** - RM: 99750  
- **Julia Lins** - RM: 98690
- **Luis Barreto** - RM: 99210
- **Victor Aranda** - RM: 99667

---

## 🎯 **CRITÉRIOS DE AVALIAÇÃO ATENDIDOS**

### ✅ ASP.NET Core Web API e Entity Framework com CRUD completo (35%)
- **Framework**: ASP.NET Core 9.0 com Entity Framework Core
- **CRUD Completo**: Operações Create, Read, Update, Delete para Investimentos e Usuários
- **Banco de Dados**: PostgreSQL com configuração para múltiplos ambientes
- **Migrations**: Configuração automática de tabelas e relacionamentos

### ✅ Pesquisas com LINQ (peso 10%)
- **Consultas LINQ Implementadas**:
  - Filtros por tipo de investimento (`Where` + `OrderByDescending`)
  - Filtros por operação (compra/venda) (`Where` + `OrderByDescending`)
  - Cálculo de saldo líquido (`Join` + `Select` + `SumAsync`)
  - Investimentos recentes (`Where` com filtro de data)
  - Dashboard com estatísticas (`GroupBy` + `Count` + `Sum` + `Average`)
  - Lista de CPFs únicos (`Join` + `Distinct` + `OrderBy`)
- **Identificação no Swagger**: Todos os endpoints LINQ estão marcados com `[LINQ]` na documentação

### ✅ Publicação em ambiente Cloud (15%)
- **Deploy**: Configurado para Render Cloud Platform
- **URLs de Produção**: Suporte para múltiplos ambientes (desenvolvimento/produção)
- **Configuração Multi-Ambiente**: Detecção automática do ambiente de execução

### ✅ Endpoints conectando com outras APIs (20%)
- **API da B3**: Integração com códigos de ações da Bolsa de Valores
- **API HG Brasil**: Consulta de cotações em tempo real
- **Validação Externa**: Sistema de validação de códigos de ações
- **HttpClient**: Configuração para consumo de APIs externas

### ✅ Documentação do projeto (10%)
- **Swagger/OpenAPI**: Documentação interativa completa
- **Exemplos**: Todos os endpoints com exemplos de uso
- **Guias de Uso**: Instruções passo a passo no próprio Swagger
- **README Detalhado**: Este documento com instruções completas

### ✅ Apresentar arquitetura em diagramas (10%)
- **Diagrama de Arquitetura**: Visualização completa do sistema
- **Fluxo de Dados**: Representação das interações entre componentes
- **Tecnologias**: Mapeamento das tecnologias utilizadas

![Diagrama de Arquitetura](Diagrama.png)

---

## ⚡ **EXECUÇÃO RÁPIDA (2 MINUTOS)**

> **� PROJETO PLUG AND PLAY - ZERO CONFIGURAÇÃO NECESSÁRIA!**

```bash
# Copie e cole estes 3 comandos no terminal:
git clone https://github.com/lgustavobarre351/SPRINT4_CSHARP_API.git
cd SPRINT4_CSHARP_API/Investimentos  
dotnet run
```

**🎯 Depois abra no navegador: `http://localhost:5171/swagger`**

> **✅ Banco de dados já configurado na nuvem (Supabase)**  
> **✅ Dados de exemplo já carregados**  
> **✅ APIs externas (B3 + HG Brasil) funcionando**  
> **✅ Todas as consultas LINQ identificadas no Swagger**

---

## �🚀 **COMO EXECUTAR O PROJETO (DETALHADO)**

### 📋 **Pré-requisitos (Mínimo)**
```bash
# Você só precisa ter instalado:
✅ .NET 9.0 SDK (obrigatório)
✅ Git (para clonar o repositório)

# NÃO PRECISA:
❌ PostgreSQL (usamos banco na nuvem)
❌ Configurar strings de conexão
❌ Executar migrations manualmente
❌ Instalar outras dependências
```

> **💡 Verificar se tem .NET 9.0**: Execute `dotnet --version` no terminal

### 🔧 **Configuração do Ambiente**

> **🎉 PROJETO PRONTO PARA EXECUTAR!** 
> 
> O banco de dados já está configurado na nuvem (Supabase). Não precisa instalar PostgreSQL nem configurar nada!

#### 1️⃣ **Clone o Repositório**
```bash
git clone https://github.com/lgustavobarre351/SPRINT4_CSHARP_API.git
cd SPRINT4_CSHARP_API
```

#### 2️⃣ **Execute o Projeto (SEM configuração adicional!)**
```bash
# Navegue para a pasta do projeto
cd Investimentos

# Execute a API - PRONTO!
dotnet run
```

> **✨ EXECUÇÃO INSTANTÂNEA:**
> - ✅ **Banco de dados**: Já configurado na nuvem (Supabase)
> - ✅ **Tabelas**: Criadas automaticamente na primeira execução
> - ✅ **Dados**: Banco compartilhado com dados de exemplo
> - ✅ **Dependências**: Restauradas automaticamente pelo .NET

**🚀 A API será iniciada em: `http://localhost:5171`**

### 🌐 **Acesso à Aplicação**

#### **🏠 Desenvolvimento Local (PLUG AND PLAY!):**
Após executar `dotnet run`, acesse:
- **🚀 API Base**: `http://localhost:5171`
- **📚 Swagger UI**: `http://localhost:5171/swagger` ← **COMECE AQUI!**
- **💡 Documentação Interativa**: Todas as funcionalidades testáveis no navegador
- **🗄️ Banco de Dados**: Conectado automaticamente na nuvem (Supabase)

> **⚡ Execução INSTANTÂNEA (3 comandos):**
> 1. `git clone https://github.com/lgustavobarre351/SPRINT4_CSHARP_API.git`
> 2. `cd SPRINT4_CSHARP_API/Investimentos`
> 3. `dotnet run`
> 
> **🎯 Pronto! API funcionando em 2 minutos!**

#### **☁️ Produção (Cloud) - Opcional:**
- **URL da API**: `https://sua-api.onrender.com` (após deploy)
- **Swagger Produção**: `https://sua-api.onrender.com/swagger`

---

## 📖 **GUIA DE USO DA API**

### 🎯 **Primeiros Passos**

**🚀 Execução SUPER Simples:**
```bash
# Apenas 3 comandos:
git clone https://github.com/lgustavobarre351/SPRINT4_CSHARP_API.git
cd SPRINT4_CSHARP_API/Investimentos
dotnet run
```

**⏱️ Em menos de 2 minutos a API estará rodando!**

**📱 Testando a API:**
1. **Aguarde**: A mensagem `"Now listening on: http://localhost:5171"`
2. **Abra o navegador**: `http://localhost:5171/swagger`
3. **Clique em "AJUDA"**: Endpoint com instruções detalhadas  
4. **Teste TUDO**: Todos os endpoints funcionando com dados reais!

### 💡 **Endpoints Principais**

#### **📊 CRUD de Investimentos**
```http
GET    /api/investimentos              # Lista todos [LINQ]
GET    /api/investimentos/{id}         # Busca por ID [LINQ]
GET    /api/investimentos/usuario/{cpf} # Por usuário [LINQ]
POST   /api/investimentos              # Criar novo
PUT    /api/investimentos/{id}         # Atualizar
DELETE /api/investimentos/{id}         # Deletar
```

#### **🔍 Consultas LINQ Especiais**
```http
GET /api/investimentos/tipo/{tipo}           # Filtrar por tipo [LINQ]
GET /api/investimentos/operacao/{operacao}   # Compras ou vendas [LINQ]
GET /api/investimentos/saldo/{cpf}           # Saldo líquido [LINQ]
GET /api/investimentos/recentes              # Últimos 30 dias [LINQ]
GET /api/investimentos/dashboard             # Estatísticas [LINQ]
GET /api/investimentos/usuarios              # CPFs únicos [LINQ]
```

#### **👥 Gerenciamento de Usuários**
```http
GET    /api/usuarios           # Listar todos [LINQ]
GET    /api/usuarios/{cpf}     # Buscar por CPF [LINQ]
POST   /api/usuarios           # Criar usuário
PUT    /api/usuarios/{cpf}     # Atualizar usuário
DELETE /api/usuarios/{cpf}     # Deletar usuário [LINQ]
```

#### **🌐 APIs Externas**
```http
GET  /api/apisexternas/codigos-b3              # Códigos B3 [LINQ]
POST /api/apisexternas/recarregar-b3           # Recarregar B3 [LINQ]
GET  /api/apisexternas/cotacao/{codigo}        # Cotação HG Brasil
GET  /api/apisexternas/validar-b3/{codigo}     # Validar código
```

### 📝 **Exemplo de Uso Completo**

#### **1. Criar um Investimento**
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

#### **2. Consultar Saldo do Usuário**
```http
GET /api/investimentos/saldo/12345678901
```

#### **3. Ver Dashboard de Investimentos**
```http
GET /api/investimentos/dashboard
```

---

## 🏗️ **ARQUITETURA DO SISTEMA**

### **📋 Componentes Principais**
- **Controllers**: Gerenciam requisições HTTP e respostas
- **Repositories**: Camada de acesso a dados com Entity Framework
- **Services**: Lógica de negócio e integração com APIs externas
- **Models**: Entidades do domínio (Investimento, UserProfile)
- **Data**: Contexto do Entity Framework e configurações de banco

### **🔄 Fluxo de Dados**
1. **Cliente** → Requisição HTTP → **Controller**
2. **Controller** → Chama → **Repository/Service**
3. **Repository** → Consulta LINQ → **Database (PostgreSQL)**
4. **Service** → Integração → **APIs Externas**
5. **Response** ← JSON ← **Controller** ← **Cliente**

### **🌍 Tecnologias Utilizadas**
- **Backend**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core
- **Banco**: PostgreSQL
- **Cloud**: Render Platform
- **APIs**: B3, HG Brasil
- **Documentação**: Swagger/OpenAPI
- **Versionamento**: Git/GitHub

---

## 🚀 **DEPLOY NA NUVEM**

### **Render Platform (Recomendado)**
```bash
# 1. Conecte seu repositório GitHub ao Render
# 2. Configure as variáveis de ambiente:
DATABASE_URL=sua_connection_string_postgresql
PORT=80

# 3. O deploy é automático a cada push na branch main
```

### **Outras Opções de Cloud**
- **Azure App Service**: Suporte nativo para .NET
- **AWS Elastic Beanstalk**: Deploy simplificado
- **Heroku**: Configuração com buildpack .NET

---

## 🧪 **TESTES E VALIDAÇÃO**

### **Testando Localmente (Zero Configuração!)**
```bash
# SUPER SIMPLES - Apenas execute:
cd Investimentos
dotnet run

# Aguarde a mensagem: "Now listening on: http://localhost:5171"
# Então abra: http://localhost:5171/swagger
```

**🎯 Tudo funcionando imediatamente:**
- ✅ **Banco conectado**: Supabase na nuvem
- ✅ **Dados disponíveis**: Investimentos e usuários de exemplo
- ✅ **APIs externas**: B3 e HG Brasil configuradas
- ✅ **Swagger completo**: Todos os endpoints testáveis

### **Validação dos Critérios**
- ✅ **CRUD**: Teste todos os endpoints de Investimentos e Usuários
- ✅ **LINQ**: Observe os comentários `[LINQ]` no Swagger
- ✅ **Cloud**: Acesse a URL de produção
- ✅ **APIs Externas**: Teste endpoints de cotação e validação B3
- ✅ **Documentação**: Navegue pelo Swagger completo
- ✅ **Diagrama**: Visualize o arquivo `Diagrama.png`

---

## 📚 **RECURSOS ADICIONAIS**

### **🔗 Links Úteis**
- **🎯 Swagger Local**: http://localhost:5171/swagger ← **ACESSE AQUI APÓS `dotnet run`**
- **📂 Repositório GitHub**: https://github.com/lgustavobarre351/SPRINT4_CSHARP_API
- **📈 API B3 Códigos**: Integração com dados reais da bolsa
- **💰 HG Brasil**: API de cotações financeiras
- **🗄️ Banco Supabase**: Conectado automaticamente (sem configuração)

> **⚡ Execução Instantânea**: `git clone + cd + dotnet run` = **API funcionando!**

### **📞 Suporte**
Para dúvidas ou problemas:
1. Consulte a documentação no Swagger
2. Verifique os logs da aplicação
3. Entre em contato com a equipe desenvolvedora

---

**🎉 Projeto desenvolvido para o Challenge FIAP 2024 - Demonstração completa de ASP.NET Core com todas as funcionalidades solicitadas!**

