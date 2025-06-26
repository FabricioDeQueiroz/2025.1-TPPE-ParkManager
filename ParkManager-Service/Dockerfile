FROM mcr.microsoft.com/dotnet/aspnet:9.0.6 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0.301 AS build
ARG BUILD_CONFIGURATION=Debug

RUN dotnet tool install --global dotnet-ef --version 9.0.3
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /src

COPY ["ParkManager-Service/ParkManager-Service.csproj", "ParkManager-Service/"]
RUN dotnet restore "ParkManager-Service/ParkManager-Service.csproj"

COPY . .
WORKDIR "/src/ParkManager-Service/"

RUN chmod +x "entrypoint.sh"

ENTRYPOINT ["/bin/bash", "entrypoint.sh"]