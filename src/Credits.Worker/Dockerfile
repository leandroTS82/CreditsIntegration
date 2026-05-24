FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Credits.Worker/Credits.Worker.csproj", "src/Credits.Worker/"]
COPY ["src/Credits.Application/Credits.Application.csproj", "src/Credits.Application/"]
COPY ["src/Credits.Domain/Credits.Domain.csproj", "src/Credits.Domain/"]
COPY ["src/Credits.Infrastructure/Credits.Infrastructure.csproj", "src/Credits.Infrastructure/"]

RUN dotnet restore "src/Credits.Worker/Credits.Worker.csproj"

COPY . .

RUN dotnet build "src/Credits.Worker/Credits.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Credits.Worker/Credits.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Credits.Worker.dll"]