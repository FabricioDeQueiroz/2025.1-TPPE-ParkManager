#!/bin/bash

DB_HOST=${DB_HOST:-db}
DB_PORT=${DB_PORT:-5432}

MAX_RETRIES=30
RETRY_COUNT=0

echo "Esperando DB aceitar conexões..."
until (echo > /dev/tcp/$DB_HOST/$DB_PORT) >/dev/null 2>&1 || [ $RETRY_COUNT -ge $MAX_RETRIES ]; do
  echo "DB não disponível - dormindo por 2 segundos..."
  sleep 2
  RETRY_COUNT=$((RETRY_COUNT+1))
done

if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
    echo "DB não ficou disponível a tempo. Saindo."
    exit 1
fi

if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo "DB está disponível. Aplicando migrations com Fakes para testes..."

  dotnet ef database update --project ParkManager-Service.csproj

  echo "Migrations aplicadas com sucesso. Iniciando a aplicação..."
elif [ "$ASPNETCORE_ENVIRONMENT" = "Production" ]; then
  echo "DB está disponível. Aplicando migrations..."

  dotnet ef database update SchemaMigration --project ParkManager-Service/ParkManager-Service.csproj

  echo "Migrations aplicadas com sucesso. Iniciando a aplicação..."
else
  echo "ASPNETCORE_ENVIRONMENT não setado ou não definido. Aplicando migrations padrão..."

  dotnet ef database update SchemaMigration --project ParkManager-Service.csproj

  echo "Migrations aplicadas com sucesso. Iniciando a aplicação..."
fi

if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo "Iniciando aplicação em modo de desenvolvimento com dotnet watch run..."

  dotnet test ParkManager-Service.csproj -l 'console;verbosity=normal' --logger:trx
  dotnet watch --project ParkManager-Service.csproj run --urls=http://0.0.0.0:8080
elif [ "$ASPNETCORE_ENVIRONMENT" = "Production" ]; then
  echo "Iniciando aplicação em modo de produção..."

  dotnet ParkManager-Service.dll
else
  echo "ASPNETCORE_ENVIRONMENT não setado ou não definido. Iniciando aplicação em modo padrão..."
  
  dotnet ParkManager-Service.dll
fi