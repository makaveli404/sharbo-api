FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY SharboAPI/SharboAPI.csproj SharboAPI/
COPY SharboAPI.Application/SharboAPI.Application.csproj SharboAPI.Application/
COPY SharboAPI.Domain/SharboAPI.Domain.csproj SharboAPI.Domain/
COPY SharboAPI.Infrastructure/SharboAPI.Infrastructure.csproj SharboAPI.Infrastructure/
RUN dotnet restore SharboAPI/SharboAPI.csproj

COPY . .
RUN dotnet publish SharboAPI/SharboAPI.csproj -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "SharboAPI.dll"]
