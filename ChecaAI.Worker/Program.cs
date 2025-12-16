using Microsoft.EntityFrameworkCore;
using ChecaAI.Infrastructure.Data;
using ChecaAI.Worker.Configuration;
using ChecaAI.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddEventLog(); // For Windows Event Log

// Configure options
builder.Services.Configure<DataSyncOptions>(
    builder.Configuration.GetSection(DataSyncOptions.SectionName));
builder.Services.Configure<SenateApiOptions>(
    builder.Configuration.GetSection(SenateApiOptions.SectionName));

// Configure Entity Framework
builder.Services.AddDbContext<ChecaAIDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure HTTP clients
builder.Services.AddHttpClient<ISenateScrapperService, SenateScrapperService>(client =>
{
    var baseUrl = builder.Configuration["SenateApi:BaseUrl"] ?? "https://legis.senado.leg.br/dadosabertos";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("User-Agent", "ChecaAI-Worker/1.0");
    
    if (TimeSpan.TryParse(builder.Configuration["SenateApi:RequestTimeout"], out var timeout))
    {
        client.Timeout = timeout;
    }
});

// Configure application services
builder.Services.AddScoped<ISenateScrapperService, SenateScrapperService>();
builder.Services.AddScoped<IDataPersistenceService, DataPersistenceService>();

// Configure hosted services
builder.Services.AddHostedService<SenateDataSyncService>();

// Build and configure host
var host = builder.Build();

// Ensure database is created and up to date
using (var scope = host.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ChecaAIDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Checking database connectivity...");
        
        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("Database connectivity confirmed");
            
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying pending migrations: {Migrations}", 
                    string.Join(", ", pendingMigrations));
                await context.Database.MigrateAsync();
            }
            else
            {
                logger.LogInformation("Database is up to date");
            }
        }
        else
        {
            logger.LogError("Cannot connect to database. Please check connection string and ensure PostgreSQL is running");
            throw new InvalidOperationException("Database connectivity failed");
        }
    }
    catch (Exception ex)
    {
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to initialize database");
        throw;
    }
}

// Log application startup
var appLogger = host.Services.GetRequiredService<ILogger<Program>>();
appLogger.LogInformation("ChecaAI Senate Data Worker started successfully");

// Run the application
try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    appLogger.LogCritical(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    appLogger.LogInformation("ChecaAI Senate Data Worker stopped");
}
