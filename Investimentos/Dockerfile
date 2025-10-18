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

# Configurar usuário não-root para segurança
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

# Expor porta
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/swagger || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Investimentos.dll"]