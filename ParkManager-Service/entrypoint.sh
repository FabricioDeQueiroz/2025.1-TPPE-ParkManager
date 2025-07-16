#!/bin/bash

DB_HOST=${DB_HOST:-db}
DB_PORT=${DB_PORT:-5432}

MAX_RETRIES=30
RETRY_COUNT=0

echo -e "\nEsperando DB aceitar conexões...\n\n"
until (echo > /dev/tcp/$DB_HOST/$DB_PORT) >/dev/null 2>&1 || [ $RETRY_COUNT -ge $MAX_RETRIES ]; do
  echo -e "\nDB não disponível - dormindo por 2 segundos...\n\n"
  sleep 2
  RETRY_COUNT=$((RETRY_COUNT+1))
done

if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
    echo -e "\nDB não ficou disponível a tempo. Saindo.\n\n"
    exit 1
fi

if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo -e "\nDB está disponível. Aplicando migrations com Fakes para testes...\n\n"

  dotnet ef database update --project ParkManager-Service.csproj

  echo -e "\nMigrations aplicadas com sucesso. Iniciando a aplicação...\n\n"
elif [ "$ASPNETCORE_ENVIRONMENT" = "Production" ]; then
  echo -e "\nDB está disponível. Aplicando migrations...\n\n"

  dotnet ef database update --project ParkManager-Service.csproj

  echo -e "\nMigrations aplicadas com sucesso. Iniciando a aplicação...\n\n"
else
  echo -e "\nASPNETCORE_ENVIRONMENT não setado ou não definido. Aplicando migrations padrão...\n\n"

  dotnet ef database update SchemaMigration --project ParkManager-Service.csproj

  echo -e "\nMigrations aplicadas com sucesso. Iniciando a aplicação...\n\n"
fi

if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo -e "\033[1;31m\nIniciando Testes:\n\n\033[0m"
  dotnet test ParkManager-Service.csproj -l 'console;verbosity=normal' --logger:trx
  
  echo -e "\033[1;31m\nVerificando formatação do código:\n\n\033[0m"
  dotnet format ParkManager-Service.csproj --verify-no-changes --verbosity d --no-restore
  
  echo -e "\033[1;33m\nIniciando aplicação em modo de desenvolvimento com dotnet watch run:\n\n\033[0m"
  dotnet watch --project ParkManager-Service.csproj run --urls=http://0.0.0.0:8080
elif [ "$ASPNETCORE_ENVIRONMENT" = "Production" ]; then
  echo -e "\nIniciando aplicação em modo de produção...\n\n"

  dotnet ParkManager-Service.dll
else
  echo -e "\nASPNETCORE_ENVIRONMENT não setado ou não definido. Iniciando aplicação em modo padrão...\n\n"
  
  dotnet ParkManager-Service.dll
fi