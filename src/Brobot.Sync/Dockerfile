FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY Brobot.sln Brobot.sln
COPY src/Brobot.Core/Brobot.Core.csproj src/Brobot.Core/Brobot.Core.csproj
COPY src/Brobot.Api/Brobot.Api.csproj src/Brobot.Api/Brobot.Api.csproj
COPY src/Brobot.Sync/Brobot.Sync.csproj src/Brobot.Sync/Brobot.Sync.csproj

RUN dotnet restore Brobot.sln

COPY . .
WORKDIR /app/src/Brobot.Sync
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "Brobot.Sync.dll"]
