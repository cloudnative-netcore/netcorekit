FROM microsoft/dotnet:2.1.3-aspnetcore-runtime-alpine AS base
WORKDIR /app

ENV ASPNETCORE_URLS http://+:5003
EXPOSE 5003

FROM microsoft/dotnet:2.1.401-sdk-alpine AS build
WORKDIR .
COPY . .

WORKDIR /samples/WebNotifier

RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebNotifier.dll"]
