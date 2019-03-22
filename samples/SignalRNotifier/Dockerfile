FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-alpine AS base
WORKDIR /app

ENV ASPNETCORE_URLS http://+:80
EXPOSE 80

FROM microsoft/dotnet:2.2.100-sdk-alpine AS build
WORKDIR .
COPY . .

WORKDIR /samples/SignalRNotifier

RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NetCoreKit.Samples.SignalRNotifier.dll"]
