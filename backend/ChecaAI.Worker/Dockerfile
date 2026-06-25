FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ChecaAI.Worker/ChecaAI.Worker.csproj", "ChecaAI.Worker/"]
COPY ["ChecaAI.Application/ChecaAI.Application.csproj", "ChecaAI.Application/"]
COPY ["ChecaAI.Domain/ChecaAI.Domain.csproj", "ChecaAI.Domain/"]
COPY ["ChecaAI.Infrastructure/ChecaAI.Infrastructure.csproj", "ChecaAI.Infrastructure/"]
RUN dotnet restore "ChecaAI.Worker/ChecaAI.Worker.csproj"
COPY . .
RUN dotnet publish "ChecaAI.Worker/ChecaAI.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChecaAI.Worker.dll"]
