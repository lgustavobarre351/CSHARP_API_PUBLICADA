#!/bin/bash
# Script de Deploy Automático para Railway/Render

echo "🚀 Iniciando processo de deploy..."

# Verificar se estamos em um repositório git
if [ ! -d ".git" ]; then
    echo "📂 Inicializando repositório Git..."
    git init
    git branch -M main
fi

# Adicionar arquivos
echo "📁 Adicionando arquivos ao Git..."
git add .

# Commit
echo "💾 Fazendo commit..."
read -p "Digite a mensagem do commit (ou Enter para usar padrão): " commit_msg
if [ -z "$commit_msg" ]; then
    commit_msg="Deploy: $(date '+%Y-%m-%d %H:%M')"
fi
git commit -m "$commit_msg"

# Verificar se origin existe
if ! git remote get-url origin > /dev/null 2>&1; then
    echo "🔗 Configure o repositório remoto:"
    echo "git remote add origin https://github.com/SEU-USUARIO/NOME-REPO.git"
    exit 1
fi

# Push
echo "⬆️ Enviando para GitHub..."
git push -u origin main

echo ""
echo "✅ Deploy preparado com sucesso!"
echo ""
echo "🌐 Próximos passos:"
echo "1. Acesse railway.app ou render.com"
echo "2. Conecte seu repositório GitHub"
echo "3. Configure as variáveis de ambiente:"
echo "   - ASPNETCORE_ENVIRONMENT=Production"
echo "   - PORT (automático na maioria das plataformas)"
echo "   - DefaultConnection (URL do PostgreSQL)"
echo ""
echo "📋 Sua API terá Swagger disponível em: /swagger"
echo ""
echo "🆘 Problemas? Consulte o DEPLOY-GUIDE.md"