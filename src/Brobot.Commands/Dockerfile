FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY Brobot.sln Brobot.sln
COPY src/Brobot.Core/Brobot.Core.csproj src/Brobot.Core/Brobot.Core.csproj
COPY src/Brobot.Api/Brobot.Api.csproj src/Brobot.Api/Brobot.Api.csproj
COPY src/Brobot.Sync/Brobot.Sync.csproj src/Brobot.Sync/Brobot.Sync.csproj
COPY src/Brobot.Monitor/Brobot.Monitor.csproj src/Brobot.Monitor/Brobot.Monitor.csproj
COPY src/Brobot.Commands/Brobot.Commands.csproj src/Brobot.Commands/Brobot.Commands.csproj
COPY src/Brobot.Jobs/Brobot.Jobs.csproj src/Brobot.Jobs/Brobot.Jobs.csproj

RUN dotnet restore Brobot.sln

COPY . .
WORKDIR /app/src/Brobot.Commands
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "Brobot.Commands.dll"]
