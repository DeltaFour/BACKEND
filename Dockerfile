# --- Etapa 1: Build da aplicação .NET 8 ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o /app/publish

# --- Etapa 2: Container final com runtime .NET + Python 3.11 ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Instala dependências do sistema
RUN apt-get update && apt-get install -y \
    python3.11 \
    python3.11-venv \
    python3.11-dev \
    cmake \
    build-essential \
    libboost-all-dev \
    libgtk-3-dev \
    libatlas-base-dev \
    && rm -rf /var/lib/apt/lists/*

# Cria virtualenv para instalar pacotes Python
RUN python3.11 -m venv /opt/pyenv
ENV PATH="/opt/pyenv/bin:$PATH"

# Atualiza pip e instala dependências Python na virtualenv
COPY FunctionPython/requirements.txt ./
RUN pip install --upgrade pip
RUN pip install --no-cache-dir -r requirements.txt

# Copia os arquivos Python da aplicação
COPY FunctionPython/ ./FunctionPython/
COPY app.pub /app.pub
COPY app.key /app.key

# Copia a aplicação .NET publicada
COPY --from=build /app/publish .

# Variáveis de ambiente para Python.NET
ENV PYTHONNET_PYDLL=/usr/lib/x86_64-linux-gnu/libpython3.11.so
ENV FUNCTION_PYTHON_PATH=/app/FunctionPython
ENV ALLOWED_HOST=http://localhost:3000

# Expõe a porta da API
EXPOSE 8080

# Entry point da aplicação
ENTRYPOINT ["dotnet", "DeltaFour.API.dll"]
