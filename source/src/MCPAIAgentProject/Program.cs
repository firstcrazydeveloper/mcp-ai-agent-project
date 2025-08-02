using MCPAIAgentProject;
using MCPAIAgentProject.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    //.ConfigureAppConfiguration((context, config) =>
    //{
    //    config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    //          .AddEnvironmentVariables();
    //})
    .ConfigureServices((context, services) =>
    {
        var configSection = context.Configuration.GetSection("AzureOpenAI");
        //services.Configure<AzureOpenAIConfig>(configSection);
        services.AddSingleton<IAzureOpenAIConfig>(sp => configSection.Get<AzureOpenAIConfig>()!);
        ////services.AddHttpClient<IAzureOpenAIConfig, AzureOpenAIConfig>();
        services.Configure<AzureOpenAIConfig>(context.Configuration.GetSection("AzureOpenAI"));
        services.AddHttpClient<IMCPAPIAgentConfig, MCPAPIAgentConfig>();
        services.AddHttpClient<IAzureOpenAIService, AzureOpenAIService>();

        var openAIConfig = context.Configuration.GetSection("AzureOpenAI").Get<AzureOpenAIConfig>();
        if (string.IsNullOrWhiteSpace(openAIConfig?.Endpoint) ||
            string.IsNullOrWhiteSpace(openAIConfig?.Key) ||
            string.IsNullOrWhiteSpace(openAIConfig?.DeploymentName))
        {
            throw new InvalidOperationException("AzureOpenAIConfig is missing required values in ApplicationConfig section.");
        }
    })
    .Build();

host.Run();
