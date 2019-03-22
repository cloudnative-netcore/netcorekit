FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-alpine AS base
RUN apk update && apk add libc6-compat
RUN GRPC_HEALTH_PROBE_VERSION=v0.2.0 && \
    wget -qO/bin/grpc_health_probe https://github.com/grpc-ecosystem/grpc-health-probe/releases/download/${GRPC_HEALTH_PROBE_VERSION}/grpc_health_probe-linux-amd64 && \
    chmod +x /bin/grpc_health_probe
WORKDIR /app

ARG service_version
ENV SERVICE_VERSION ${service_version:-0.0.1}

ARG api_version
ENV API_VERSION ${api_version:-1.0}

FROM microsoft/dotnet:2.2.100-sdk-alpine AS build
WORKDIR .
COPY . .

WORKDIR /samples/ExchangeService

RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NetCoreKit.Samples.ExchangeService.dll"]
