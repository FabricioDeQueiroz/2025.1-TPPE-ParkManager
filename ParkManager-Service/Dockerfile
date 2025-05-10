FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ParkManager-Service/ParkManager-Service.csproj", "ParkManager-Service/"]
RUN dotnet restore "ParkManager-Service/ParkManager-Service.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet restore "ParkManager-Service/ParkManager-Service.csproj"
RUN dotnet build "ParkManager-Service/ParkManager-Service.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet test "ParkManager-Service/ParkManager-Service.csproj" -l "console;verbosity=detailed" --logger:trx
CMD ["/bin/sh", "-c", "dotnet watch --project ParkManager-Service.csproj run --urls=http://0.0.0.0:8080"]

# Usar futuramente para Docker de build
# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "ParkManager-Service/ParkManager-Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "ParkManager-Service.dll"]