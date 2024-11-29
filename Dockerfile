FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY SharboAPI.sln .
COPY SharboAPI/SharboAPI.csproj SharboAPI/
COPY SharboAPI.Application/SharboAPI.Application.csproj SharboAPI.Application/
COPY SharboAPI.Domain/SharboAPI.Domain.csproj SharboAPI.Domain/
COPY SharboAPI.Infrastructure/SharboAPI.Infrastructure.csproj SharboAPI.Infrastructure/
COPY SharboAPI.Domain.Tests/SharboAPI.Domain.Tests.csproj SharboAPI.Domain.Tests/
RUN dotnet restore
COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish SharboAPI/SharboAPI.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SharboAPI.dll"]
