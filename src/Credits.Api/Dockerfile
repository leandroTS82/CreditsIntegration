FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Credits.Api/Credits.Api.csproj", "src/Credits.Api/"]
COPY ["src/Credits.Application/Credits.Application.csproj", "src/Credits.Application/"]
COPY ["src/Credits.Domain/Credits.Domain.csproj", "src/Credits.Domain/"]
COPY ["src/Credits.Infrastructure/Credits.Infrastructure.csproj", "src/Credits.Infrastructure/"]

RUN dotnet restore "src/Credits.Api/Credits.Api.csproj"

COPY . .

RUN dotnet build "src/Credits.Api/Credits.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Credits.Api/Credits.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Credits.Api.dll"]