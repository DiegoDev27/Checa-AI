namespace ChecaAI.Worker.Configuration;

public class DataSyncOptions
{
    public const string SectionName = "DataSync";

    public TimeSpan SenateDataSyncInterval { get; set; } = TimeSpan.FromHours(2);
    
    public bool EnableScheduledSync { get; set; } = true;
    
    public TimeSpan SyncStartTime { get; set; } = TimeSpan.FromHours(6);
    
    public int RetryAttempts { get; set; } = 3;
    
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(1);
}

public class SenateApiOptions
{
    public const string SectionName = "SenateApi";

    public string BaseUrl { get; set; } = "https://legis.senado.leg.br/dadosabertos";
    
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(5);
}