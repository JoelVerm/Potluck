FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0 AS build-env
WORKDIR /app

COPY ./ ./
RUN dotnet publish -a $TARGETARCH Main -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0-distroless-extra
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Potluck.dll"]
