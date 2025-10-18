# 🚀 Guia de Deploy - API Investimentos

## 🎯 Plataformas Gratuitas SEM HIBERNAÇÃO (24/7)

### 1. 🟣 **Railway.app** ⭐⭐⭐⭐⭐ (MAIS RECOMENDADO)
**Por que escolher:**
- ✅ **SEM hibernação forçada** - API fica online 24/7
- ✅ **$5 USD crédito mensal** (renova automaticamente todo mês)
- ✅ **PostgreSQL gratuito** incluído (1GB)
- ✅ **500 horas/mês gratuitas** (suficiente para rodar 24/7)
- ✅ **Deploy automático** via GitHub
- ✅ **Suporte nativo ao .NET**
- 💰 **Custo real: ~$1-2 USD/mês** (bem dentro do crédito gratuito)

**Como fazer deploy:**

1. **Preparar GitHub:**
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin https://github.com/SEU-USUARIO/NOME-REPO.git
   git push -u origin main
   ```

2. **Railway Deploy:**
   - Acesse [railway.app](https://railway.app)
   - Conecte sua conta GitHub
   - Clique em "New Project" → "Deploy from GitHub repo"
   - Selecione seu repositório
   - Adicione PostgreSQL: "Add Service" → "Database" → "PostgreSQL"

3. **Variáveis de Ambiente (Railway):**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   PORT=${{RAILWAY_PUBLIC_PORT}}
   DefaultConnection=${{Postgres.DATABASE_URL}}
   ```

---

### 2. � **Fly.io** ⭐⭐⭐⭐
**Por que escolher:**
- ✅ **SEM hibernação forçada**
- ✅ **3 apps gratuitas** permanentemente
- ✅ **160GB bandwidth/mês**
- ✅ **PostgreSQL gratuito**
- ✅ **Performance excelente** (rede global)
- ✅ **CLI poderosa** para deploy

---

### 3. 🟨 **Koyeb** ⭐⭐⭐⭐
**Por que escolher:**
- ✅ **SEM hibernação**
- ✅ **Tier gratuito permanente**
- ✅ **512MB RAM, 1 vCPU**
- ✅ **Deploy via GitHub/Docker**
- ✅ **Edge locations** globais

---

### ❌ **EVITAR: Render.com (Tier Gratuito)**
**Problemas:**
- ❌ **Hibernação forçada** após 15 minutos de inatividade
- ❌ **Cold start** lento (até 30 segundos para "acordar")
- ❌ **Indisponibilidade** durante hibernação

**Como fazer deploy:**

1. **Preparar GitHub** (mesmo processo acima)

2. **Render Deploy:**
   - Acesse [render.com](https://render.com)
   - "New" → "Web Service"
   - Conecte GitHub e selecione repo
   - **Configurações:**
     - Build Command: `dotnet publish Investimentos/Investimentos.csproj -c Release -o out`
     - Start Command: `dotnet out/Investimentos.dll`
     - Environment: `Production`

3. **PostgreSQL no Render:**
   - "New" → "PostgreSQL" (Free tier)
   - Copie a `Database URL`

4. **Variáveis de Ambiente:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   PORT=10000
   DefaultConnection=SUA_DATABASE_URL_DO_POSTGRESQL
   ```

---

### 3. 🟦 **Fly.io**
**Por que escolher:**
- ✅ Tier gratuito generoso (3 apps gratuitas)
- ✅ PostgreSQL gratuito
- ✅ Sem hibernação forçada
- ✅ Performance excelente

**Como fazer deploy:**

1. **Instalar Fly CLI:**
   ```powershell
   # Windows
   iwr https://fly.io/install.ps1 -useb | iex
   ```

2. **Deploy:**
   ```bash
   fly auth login
   fly launch --copy-config --name sua-api-investimentos
   ```

3. **PostgreSQL:**
   ```bash
   fly postgres create --name sua-api-db
   fly postgres attach sua-api-db -a sua-api-investimentos
   ```

---

## 🛠️ Configurações do Projeto

### ✅ Arquivos já configurados:
- `Dockerfile` - Otimizado para produção
- `railway.json` - Configuração Railway
- `appsettings.Production.json` - Config produção
- `Program.cs` - Multi-ambiente configurado

### 🔧 Checklist antes do deploy:
- [ ] Código commitado no GitHub
- [ ] Dockerfile funcional
- [ ] Variáveis de ambiente configuradas
- [ ] Connection string apontando para PostgreSQL cloud
- [ ] CORS configurado (já está)
- [ ] Swagger habilitado em produção (já está)

---

## 🌐 URLs após deploy:

**Railway:** `https://SEU-APP-production.up.railway.app`
**Render:** `https://SEU-APP.onrender.com` 
**Fly.io:** `https://SEU-APP.fly.dev`

### 📋 Swagger URL:
Adicione `/swagger` no final da sua URL de produção.

---

## 🆘 Troubleshooting

### Erro de conexão PostgreSQL:
```bash
# Verificar connection string
echo $DefaultConnection

# Formato correto PostgreSQL:
# postgresql://usuario:senha@host:5432/database
```

### App não inicia:
```bash
# Verificar logs
# Railway: Aba "Deployments" → Click no deployment → "View Logs"
# Render: Aba "Logs"
# Fly: fly logs
```

### Porta incorreta:
- Railway: Use `${{RAILWAY_PUBLIC_PORT}}`  
- Render: Use `PORT=10000`
- Fly: Use `PORT=8080`

---

## 💡 Dicas importantes:

1. **PostgreSQL Connection String**: Será fornecida automaticamente pela plataforma
2. **CORS**: Já configurado para aceitar qualquer origem
3. **Swagger**: Habilitado em produção para demonstração
4. **Health Check**: Configurado no Dockerfile
5. **SSL**: Automático em todas as plataformas

---

## 🎯 Próximos passos após deploy:

1. Teste todos os endpoints via Swagger
2. Configure CI/CD para deploys automáticos
3. Monitore logs de erro
4. Configure backup do banco de dados
5. Implemente rate limiting se necessário

Boa sorte com o deploy! 🚀