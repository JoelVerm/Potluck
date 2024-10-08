FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0-arm64v8 AS build-env
WORKDIR /app

COPY ./Main ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0-distroless-extra-arm64v8
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Potluck.dll"]
