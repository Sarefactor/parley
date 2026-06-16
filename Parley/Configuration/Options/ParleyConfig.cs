using Parley.Configuration.Enums;

namespace Parley.Configuration.Options;

public class ParleyConfig
{
    public static readonly string AppsettingsKey = "Parley";

    public AgentProviderType AgentProvider { get; set; } = 0;

    public OpenAiConfig OpenAiConfig { get; set; } = new();
    public ParleyDatabases Databases { get; set; } = new();
}

public class OpenAiConfig
{
    public string ApiKey { get; set; } = default!;
    public string Model { get; set; } = default!;
}

public class ParleyDatabases
{
    public ParleyMongoDb MongoDb { get; set; } = new();
}

public class ParleyMongoDb
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string AgentSchemaCollection { get; set; } = string.Empty;
    public string ConfigurationCollection { get; set; } = string.Empty;
}