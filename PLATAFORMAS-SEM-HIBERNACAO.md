# 🏆 TOP 3 Plataformas Gratuitas SEM Hibernação

## 🥇 1. Railway.app - MAIS FÁCIL E CONFIÁVEL

### ✅ **Vantagens:**
- **NUNCA hiberna** sua API
- **$5 USD/mês** de crédito gratuito (renova automaticamente)
- **PostgreSQL incluído** automaticamente
- **Interface super simples**
- **Deploy automático** a cada push no GitHub

### 🚀 **Como fazer deploy:**

1. **Preparar GitHub:**
   ```powershell
   # Execute o script de deploy
   .\deploy.ps1
   ```

2. **Railway Deploy:**
   - Acesse https://railway.app
   - Clique "Login with GitHub"
   - "New Project" → "Deploy from GitHub repo"
   - Selecione seu repositório
   - Adicione PostgreSQL: "New" → "Database" → "Add PostgreSQL"

3. **Variáveis (Railway configura automaticamente):**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   PORT=${{RAILWAY_PUBLIC_PORT}}
   DefaultConnection=${{Postgres.DATABASE_URL}}
   ```

4. **✅ Pronto!** Sua API estará online em: `https://seu-app-production.up.railway.app`

---

## 🥈 2. Fly.io - PERFORMANCE MÁXIMA

### ✅ **Vantagens:**
- **Sem hibernação**
- **3 apps gratuitas** para sempre
- **Rede global** (servidores no mundo todo)
- **PostgreSQL gratuito**

### 🚀 **Como fazer deploy:**

1. **Instalar Fly CLI:**
   ```powershell
   # Instalar via PowerShell
   iwr https://fly.io/install.ps1 -useb | iex
   ```

2. **Deploy:**
   ```bash
   # Login
   fly auth login
   
   # Criar app
   fly launch --copy-config --name sua-api-investimentos
   
   # Adicionar PostgreSQL
   fly postgres create --name sua-api-db
   fly postgres attach sua-api-db -a sua-api-investimentos
   
   # Deploy
   fly deploy
   ```

3. **✅ Pronto!** Sua API estará online em: `https://sua-api-investimentos.fly.dev`

---

## 🥉 3. Koyeb - ALTERNATIVA SÓLIDA

### ✅ **Vantagens:**
- **Sem hibernação**
- **Gratuito permanente**
- **Deploy via GitHub**
- **Interface simples**

### 🚀 **Como fazer deploy:**

1. **Preparar GitHub** (execute `.\deploy.ps1`)

2. **Koyeb Deploy:**
   - Acesse https://koyeb.com
   - "Create Account" → Login com GitHub
   - "Create App" → "GitHub repository"
   - Selecione seu repositório
   - **Build settings:**
     - Build command: `docker build -t app .`
     - Run command: `docker run -p 8000:8080 app`

3. **PostgreSQL separado (Neon.tech):**
   - Acesse https://neon.tech (PostgreSQL gratuito)
   - Crie database gratuito
   - Copie connection string
   - Cole nas variáveis do Koyeb: `DefaultConnection=sua_connection_string`

---

## 💡 **Recomendação Final:**

### 🏆 **Use Railway.app se:**
- Quer o **mais simples** e confiável
- Não quer se preocupar com configurações
- Prefere tudo integrado (PostgreSQL + API)

### 🚀 **Use Fly.io se:**
- Quer **máxima performance**
- Gosta de CLI e controle avançado
- Precisa de múltiplas regiões

### 🔧 **Use Koyeb se:**
- Quer uma alternativa ao Railway
- Não se importa de configurar PostgreSQL separado

---

## ⚠️ **IMPORTANTE:**

Todas essas plataformas **NÃO hibernam** no tier gratuito, ao contrário do Render. Sua API ficará **online 24/7** sem interrupções!

## 🆘 **Precisa de ajuda?**

Fale qual plataforma quer usar e te ajudo com o deploy passo a passo!