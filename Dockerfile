FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /tools
COPY --from=build /tools .
WORKDIR /app
COPY ./publish ./
ENTRYPOINT ["dotnet", "MercuryIntegrationService.dll"]