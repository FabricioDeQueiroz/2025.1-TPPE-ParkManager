# Makefile ParkManager
all: 
	@echo "Use um comando válido"

docker:
	docker compose -p parkmanager down
	docker-compose -p parkmanager up --build -d

docker-down:
	docker compose -p parkmanager down

docker-build:
	docker-compose -p parkmanager up --build -d

docker-lint:
	docker exec -it parkmanager_service dotnet format ParkManager-Service.csproj --verify-no-changes --verbosity d --no-restore

docker-test:
	docker exec -it parkmanager_service dotnet test -l "console;verbosity=detailed"