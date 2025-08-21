namespace BTG.Infrastructure.Persistence;

public class MongoOptions
{
    public const string SectionName = "Mongo"; // mapear en appsettings

    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
}

