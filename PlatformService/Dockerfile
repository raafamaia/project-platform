FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-dev
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-dev /app/out .
ENTRYPOINT ["dotnet", "PlatformService.dll"]