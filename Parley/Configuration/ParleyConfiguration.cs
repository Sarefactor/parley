using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parley.Classification;
using Parley.Configuration.Options;
using Parley.Core.DataAccess.Factories;
using Parley.Core.DataAccess.Repositories;
using Parley.Core.Services;
using Parley.Mappers;
using Parley.Providers;
using Parley.Workflows.Nodes.Factories;
using Parley.Workflows.State;
using Parley.Workflows.Validation;

namespace Parley.Configuration;

public static class ParleyConfiguration
{
    public static IServiceCollection ConfigureParley(this IServiceCollection services,
                                                     IConfiguration configuration,
                                                     bool useDefaultMongoDb)
    {
        ConfigureOptions(services, configuration);
        ConfigureServices(services, configuration);
        ConfigureState(services, configuration);
        ConfigureDatabases(services, configuration, useDefaultMongoDb);

        return services;
    }

    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ParleyConfig>(configuration.GetSection(ParleyConfig.AppsettingsKey));
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient().AddLogging();
        services.AddAGUI();

        services.AddSingleton<IAgentProvider, AgentProvider>();
        services.AddSingleton<ISessionProvider, SessionProvider>();
        services.AddSingleton<ISchemaProvider, SchemaProvider>();
        services.AddSingleton<IChatClientProvider, ChatClientProvider>();
        services.AddSingleton<IParleyNodeFactory, ParleyNodeFactory>();
        services.AddSingleton<IValidateInput, InputValidator>();
        services.AddSingleton<ITextClassifier, TextClassifier>();
        services.AddSingleton<IWorkflowClassifier, WorkflowClassifier>();
        services.AddSingleton<ISchemaFactory, SchemaFactory>();
        services.AddSingleton<IAgentSchemaRegistry, AgentSchemaRegistry>();
        services.AddSingleton<IAgentSchemaDtoMapper, AgentSchemaDtoMapper>();

        services.AddMemoryCache();
    }

    private static void ConfigureState(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IWorkflowStateManager, WorkflowStateManager>();
    }

    private static void ConfigureDatabases(IServiceCollection services, IConfiguration configuration, bool useDefaultMongoDb)
    {
        if (useDefaultMongoDb)
        { 
            services.AddSingleton<IAgentSchemaRepository, MongoDbAgentSchemaRepository>();
            services.AddSingleton<IAgentConfigurationRepository, MongoDbAgentConfigurationRepository>();
            ParleyNodeOptionsSerialiser.ConfigureMongoDbSerialisation();
        }
    }

    public static async Task PreloadNodes(WebApplication app)
    {
        var parleyNodeFactory = app.Services.GetService<IParleyNodeFactory>();
        parleyNodeFactory?.Preload();
    }

    public static async Task PreloadNodes(IHost host)
    {
        var parleyNodeFactory = host.Services.GetService<IParleyNodeFactory>();
        parleyNodeFactory?.Preload();
    }
}