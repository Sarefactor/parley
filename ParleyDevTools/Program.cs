using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parley.Configuration;
using ParleyDevTools.Application.Core.Menu;
using ParleyDevTools.Application.Services;

var builder = Host.CreateApplicationBuilder(args);

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).AddUserSecrets<Program>().Build();

RegisterServices(builder.Services, configuration);

var host = builder.Build();

await ParleyConfiguration.PreloadNodes(host);

var application = host.Services.GetService<DevToolsMenu>();

if (application != null)
    await application.Start();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    ParleyConfiguration.ConfigureParley(services, configuration, true);

    services.AddScoped<IWorkflowService, WorkflowService>()
            .AddScoped<DevToolsMenu>();
}