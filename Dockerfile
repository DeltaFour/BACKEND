# --- Etapa 1: Build da aplicação .NET 8 ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o /app/publish

# --- Etapa 2: Container final com runtime .NET + Python 3.11 ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY app.pub /app.pub
COPY app.key /app.key
COPY --from=build /app/publish .

# Expõe a porta da API
EXPOSE 8080

# Entry point da aplicação
ENTRYPOINT ["dotnet", "DeltaFour.API.dll"]
