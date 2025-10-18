# Script de Deploy para Windows PowerShell
param(
    [string]$CommitMessage = "Deploy: $(Get-Date -Format 'yyyy-MM-dd HH:mm')"
)

Write-Host "🚀 Iniciando processo de deploy..." -ForegroundColor Green

# Verificar se estamos em um repositório git
if (!(Test-Path ".git")) {
    Write-Host "📂 Inicializando repositório Git..." -ForegroundColor Yellow
    git init
    git branch -M main
}

# Adicionar arquivos
Write-Host "📁 Adicionando arquivos ao Git..." -ForegroundColor Yellow
git add .

# Commit
Write-Host "💾 Fazendo commit..." -ForegroundColor Yellow
git commit -m $CommitMessage

# Verificar se origin existe
try {
    git remote get-url origin | Out-Null
} catch {
    Write-Host "🔗 Configure o repositório remoto:" -ForegroundColor Red
    Write-Host "git remote add origin https://github.com/SEU-USUARIO/NOME-REPO.git" -ForegroundColor Cyan
    exit 1
}

# Push
Write-Host "⬆️ Enviando para GitHub..." -ForegroundColor Yellow
git push -u origin main

Write-Host ""
Write-Host "✅ Deploy preparado com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 PLATAFORMAS SEM HIBERNAÇÃO (24/7):" -ForegroundColor Cyan
Write-Host ""
Write-Host "🥇 1. Railway.app (MAIS FÁCIL)" -ForegroundColor Green
Write-Host "   - Acesse: https://railway.app" -ForegroundColor White
Write-Host "   - New Project → Deploy from GitHub repo" -ForegroundColor Gray
Write-Host "   - Add PostgreSQL → Automático!" -ForegroundColor Gray
Write-Host ""
Write-Host "🥈 2. Fly.io (PERFORMANCE)" -ForegroundColor Blue
Write-Host "   - Execute: fly auth login && fly launch" -ForegroundColor White
Write-Host "   - PostgreSQL: fly postgres create" -ForegroundColor Gray
Write-Host ""
Write-Host "🥉 3. Koyeb (ALTERNATIVA)" -ForegroundColor Yellow
Write-Host "   - Acesse: https://koyeb.com" -ForegroundColor White
Write-Host "   - PostgreSQL: https://neon.tech (separado)" -ForegroundColor Gray
Write-Host ""
Write-Host "📋 Todas terão Swagger em: /swagger" -ForegroundColor Magenta
Write-Host "📖 Guia detalhado: PLATAFORMAS-SEM-HIBERNACAO.md" -ForegroundColor Cyan
Write-Host ""
Write-Host "🆘 Dúvidas? Consulte os arquivos .md criados!" -ForegroundColor Yellow