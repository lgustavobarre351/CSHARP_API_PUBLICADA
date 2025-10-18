#!/bin/bash

# Cores para terminal
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

clear

echo -e "${BLUE}"
echo "  ╔══════════════════════════════════════════════╗"
echo "  ║          🚀 API DE INVESTIMENTOS             ║"
echo "  ║              Challenge FIAP 2024             ║"
echo "  ╚══════════════════════════════════════════════╝"
echo -e "${NC}"

echo -e "${YELLOW}📍 Verificando pasta do projeto...${NC}"
if [ ! -d "Investimentos" ]; then
    echo -e "${RED}❌ Pasta 'Investimentos' não encontrada!${NC}"
    echo -e "${YELLOW}💡 Execute este script na pasta raiz do projeto SPRINT4_CSHARP_API${NC}"
    read -p "Pressione Enter para sair..."
    exit 1
fi

echo -e "${GREEN}✅ Pasta encontrada: $(pwd)/Investimentos${NC}"
echo

echo -e "${YELLOW}📦 Restaurando dependências...${NC}"
cd Investimentos
dotnet restore
if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Erro ao restaurar dependências!${NC}"
    read -p "Pressione Enter para sair..."
    exit 1
fi

echo
echo -e "${GREEN}🚀 Iniciando API (Banco Supabase já configurado)...${NC}"
echo
echo -e "${BLUE}"
echo "  ╔══════════════════════════════════════════════╗"
echo "  ║  📋 Swagger: http://localhost:5171/swagger   ║"
echo "  ║  🌐 API:     http://localhost:5171/api       ║"
echo "  ║  🗄️ Banco:    Supabase (conectado auto)      ║"
echo "  ║  🔍 LINQ:     Identificado no Swagger        ║"
echo "  ║  ❌ Para parar: Ctrl+C                       ║"
echo "  ╚══════════════════════════════════════════════╝"
echo -e "${NC}"
echo -e "${YELLOW}💡 PLUG AND PLAY: Zero configuração necessária!${NC}"

dotnet run

echo
echo -e "${YELLOW}👋 API finalizada. Pressione Enter para sair...${NC}"
read