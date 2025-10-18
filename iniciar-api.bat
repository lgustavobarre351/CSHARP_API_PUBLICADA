@echo off
title API de Investimentos - Challenge FIAP
color 0A

echo.
echo  ╔══════════════════════════════════════════════╗
echo  ║          🚀 API DE INVESTIMENTOS             ║
echo  ║              Challenge FIAP 2024             ║
echo  ╚══════════════════════════════════════════════╝
echo.

echo 📍 Verificando pasta do projeto...
if not exist "Investimentos" (
    echo ❌ Pasta 'Investimentos' não encontrada!
    echo 💡 Execute este script na pasta raiz do projeto SPRINT4_CSHARP_API
    pause
    exit /b 1
)

echo ✅ Pasta encontrada: %cd%\Investimentos
echo.

echo 📦 Restaurando dependências...
cd Investimentos
call dotnet restore
if errorlevel 1 (
    echo ❌ Erro ao restaurar dependências!
    pause
    exit /b 1
)

echo.
echo 🚀 Iniciando API (Banco Supabase já configurado)...
echo.
echo  ╔══════════════════════════════════════════════╗
echo  ║  📋 Swagger: http://localhost:5171/swagger   ║
echo  ║  🌐 API:     http://localhost:5171/api       ║
echo  ║  🗄️ Banco:    Supabase (conectado auto)      ║
echo  ║  🔍 LINQ:     Identificado no Swagger        ║
echo  ║  ❌ Para parar: Ctrl+C                       ║
echo  ╚══════════════════════════════════════════════╝
echo.
echo 💡 PLUG AND PLAY: Zero configuração necessária!

call dotnet run

echo.
echo 👋 API finalizada. Pressione qualquer tecla para sair...
pause > nul