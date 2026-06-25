FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ChecaAI.Api/ChecaAI.Api.csproj", "ChecaAI.Api/"]
COPY ["ChecaAI.Application/ChecaAI.Application.csproj", "ChecaAI.Application/"]
COPY ["ChecaAI.Domain/ChecaAI.Domain.csproj", "ChecaAI.Domain/"]
COPY ["ChecaAI.Infrastructure/ChecaAI.Infrastructure.csproj", "ChecaAI.Infrastructure/"]
RUN dotnet restore "ChecaAI.Api/ChecaAI.Api.csproj"
COPY . .
RUN dotnet publish "ChecaAI.Api/ChecaAI.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChecaAI.Api.dll"]
