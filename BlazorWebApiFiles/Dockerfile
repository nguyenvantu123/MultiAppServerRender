# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorWebApiFiles/BlazorWebApiFiles.csproj", "BlazorWebApiFiles/"]
COPY ["Aspire.Microsoft.EntityFrameworkCore.SqlServer/Aspire.Microsoft.EntityFrameworkCore.SqlServer.csproj", "Aspire.Microsoft.EntityFrameworkCore.SqlServer/"]
COPY ["Aspire.Minio.Client/Aspire.Minio.Client.csproj", "Aspire.Minio.Client/"]
COPY ["Aspire.Pomelo.EntityFrameworkCore.MySql/Aspire.Pomelo.EntityFrameworkCore.MySql.csproj", "Aspire.Pomelo.EntityFrameworkCore.MySql/"]
COPY ["EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "EventBus.RabbitMQ/"]
COPY ["EventBus/EventBus.csproj", "EventBus/"]
COPY ["IntegrationEventLogEF/IntegrationEventLogEF.csproj", "IntegrationEventLogEF/"]
COPY ["MultiAppServer.ServiceDefaults/MultiAppServer.ServiceDefaults.csproj", "MultiAppServer.ServiceDefaults/"]
RUN dotnet restore "./BlazorWebApiFiles/BlazorWebApiFiles.csproj"
COPY . .
WORKDIR "/src/BlazorWebApiFiles"
RUN dotnet build "./BlazorWebApiFiles.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorWebApiFiles.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "BlazorWebApiFiles.dll"]