# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
ChecaAI is a .NET 8 Web API platform for Brazilian political transparency. It tracks politicians (senators, deputies, councilors), their votes, proposals, and expenses by integrating with government APIs and web scraping municipal chambers.

## Architecture
- **Clean Architecture** with separation of concerns across layers
- **ChecaAI.Api**: Web API controllers and configuration
- **ChecaAI.Application**: Business logic and services  
- **ChecaAI.Domain**: Entity models and core domain logic
- **ChecaAI.Infrastructure**: Data access, Entity Framework, external integrations
- **ChecaAI.Worker**: Background service for data scraping and synchronization

## Database
- **PostgreSQL** with Entity Framework Core 8.0.11
- **Main Entities**: Politician, Proposal, VotingSession, Vote, PoliticianExpense
- **DbContext**: ChecaAIDbContext in Infrastructure layer

## Development Commands

### Database
```bash
# Start PostgreSQL with Docker Compose
docker-compose up -d

# Create migration
dotnet ef migrations add <MigrationName> --project ChecaAI.Infrastructure --startup-project ChecaAI.Api

# Update database
dotnet ef database update --project ChecaAI.Infrastructure --startup-project ChecaAI.Api

# Stop services
docker-compose down
```

### Build and Run
```bash
# Build solution
dotnet build

# Run API
dotnet run --project ChecaAI.Api

# Run specific project
dotnet run --project <ProjectName>

# Run Worker for data synchronization
dotnet run --project ChecaAI.Worker
```

### Testing
```bash
# Run all tests (when test projects are added)
dotnet test

# Run specific test project
dotnet test <TestProjectPath>
```

## Configuration
- **Connection String**: appsettings.json DefaultConnection
- **Government APIs**: 
  - Câmara dos Deputados: https://dadosabertos.camara.leg.br/api/v2
  - Senado Federal: https://legis.senado.leg.br/dadosabertos
- **PgAdmin**: http://localhost:8080 (admin@checaai.com / admin123)

## Key Features Implementation
1. **Government API Integration**: Services to fetch data from Brazilian government transparency APIs
2. **Web Scraping**: Services for municipal chambers without public APIs  
3. **Vote Tracking**: Complete voting history and politician positions
4. **Expense Monitoring**: Political expenses and reimbursements tracking
5. **Data Synchronization**: Regular updates from multiple data sources

## Project Structure
```
├── ChecaAI.Api/              # Web API layer
├── ChecaAI.Application/       # Business logic
├── ChecaAI.Domain/            # Domain entities
│   └── Entities/              # Core entities (Politician, Vote, etc.)
├── ChecaAI.Infrastructure/    # Data access
│   └── Data/                  # DbContext and configurations
├── ChecaAI.Worker/            # Background services
│   ├── Models/DTOs/           # XML deserialization models
│   ├── Services/              # Data scraping services
│   └── Configuration/         # Worker settings
├── docker-compose.yml         # PostgreSQL setup
└── ChecaAI.sln               # Solution file
```

## Development Workflow
1. Always run `docker-compose up -d` before starting development
2. Use migrations for database changes: create migration, then update database  
3. Follow Clean Architecture principles: Domain → Application → Infrastructure → API
4. Test locally with Swagger UI at https://localhost:7001/swagger (when running)

## Data Worker (ChecaAI.Worker)
The Worker is a background service that automatically synchronizes data from government APIs.

### Features
- **Automatic Senate Data Sync**: Fetches senator data from https://legis.senado.leg.br/dadosabertos/senador/lista/atual
- **Configurable Intervals**: Sync frequency configurable via appsettings.json
- **Retry Logic**: Automatic retries with exponential backoff
- **Comprehensive Logging**: Detailed logging for monitoring and troubleshooting

### Configuration (appsettings.json)
```json
{
  "DataSync": {
    "SenateDataSyncInterval": "02:00:00",  // Every 2 hours
    "EnableScheduledSync": true,
    "SyncStartTime": "06:00:00",           // Start at 6 AM
    "RetryAttempts": 3,
    "RetryDelay": "00:01:00"
  },
  "SenateApi": {
    "BaseUrl": "https://legis.senado.leg.br/dadosabertos",
    "RequestTimeout": "00:05:00"
  }
}
```

### Usage
```bash
# Run worker once (one-time sync)
dotnet run --project ChecaAI.Worker

# Run worker as service (continuous monitoring)
# Set EnableScheduledSync: true in appsettings.json
dotnet run --project ChecaAI.Worker
```

## External Dependencies
- Microsoft.EntityFrameworkCore 8.0.11
- Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11
- Microsoft.Extensions.Http 10.0.1
- PostgreSQL 15 (via Docker)
- PgAdmin 4 (via Docker)