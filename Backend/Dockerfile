FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0-arm64v8 AS build-env
WORKDIR /app

COPY ./Main ./

ENTRYPOINT "dotnet restore && dotnet run"
