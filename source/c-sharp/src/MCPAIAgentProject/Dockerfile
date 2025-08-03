# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base runtime for Azure Functions
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 7072

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MCPAIAgentProject/MCPAIAgentProject.csproj", "MCPAIAgentProject/"]

# Restore dependencies
RUN dotnet restore "./MCPAIAgentProject/MCPAIAgentProject.csproj"

# Copy all source files
COPY . .

# Ensure local.settings.json is included in build output (for dev containers)
# Add this to your .csproj too (CopyToOutputDirectory)
WORKDIR "/src/MCPAIAgentProject"
RUN dotnet build "./MCPAIAgentProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MCPAIAgentProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage (runtime)
FROM base AS final
WORKDIR /home/site/wwwroot

# Copy published build
COPY --from=publish /app/publish .

# Copy local.settings.json explicitly (for local/dev containers)
# NOTE: Do not use this in production images containing secrets
COPY MCPAIAgentProject/local.settings.json ./local.settings.json

# Azure Functions settings
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
