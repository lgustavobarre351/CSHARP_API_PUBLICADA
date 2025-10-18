# Multi-stage build para produção otimizada
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy projeto e restaurar dependências
COPY Investimentos/Investimentos.csproj Investimentos/
WORKDIR /src/Investimentos
RUN dotnet restore

# Copy source code e build
WORKDIR /src
COPY Investimentos/ Investimentos/
WORKDIR /src/Investimentos
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage - imagem menor
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Instalar dependências necessárias
RUN apt-get update && apt-get install -y \
    ca-certificates \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy aplicação
COPY --from=build /app/publish .

# Configurar variáveis de ambiente para Railway
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Configurar usuário não-root para segurança (mas só em desenvolvimento)
# Railway funciona melhor como root em alguns casos
# RUN groupadd -r appuser && useradd -r -g appuser appuser
# RUN chown -R appuser:appuser /app
# USER appuser

# Expor porta (Railway usa PORT dinâmico, mas padrão é 8080)
EXPOSE 8080

# Health check mais robusto
HEALTHCHECK --interval=30s --timeout=15s --start-period=90s --retries=5 \
    CMD curl -f http://localhost:8080/health || curl -f http://localhost:8080/swagger || exit 1

# Entry point com logs mais verbosos
ENTRYPOINT ["dotnet", "Investimentos.dll", "--environment=Production"]