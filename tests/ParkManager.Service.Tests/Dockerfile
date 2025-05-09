FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["tests/ParkManager.Service.Tests/ParkManager.Service.Tests.csproj", "tests/ParkManager.Service.Tests/"]
COPY ["ParkManager-Service/ParkManager-Service.csproj", "ParkManager-Service/"]

RUN dotnet restore "tests/ParkManager.Service.Tests/ParkManager.Service.Tests.csproj"

COPY ./ParkManager-Service ./ParkManager-Service
COPY . ./tests/ParkManager.Service.Tests

WORKDIR "/src/tests/ParkManager.Service.Tests"

CMD ["dotnet", "test", "-l", "console;verbosity=detailed", "--logger:trx"]