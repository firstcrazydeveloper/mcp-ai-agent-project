using MCPAIAgentProject;
using MCPAIAgentProject.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        string filePath = @"local.settings.json";
        if (!File.Exists(filePath))
        {
            throw new InvalidOperationException("Config file is missing.");
        }
        config.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configSection = context.Configuration.GetSection("AzureOpenAI");
        //services.Configure<AzureOpenAIConfig>(configSection);
        services.AddSingleton<IAzureOpenAIConfig>(sp => configSection.Get<AzureOpenAIConfig>()!);
        ////services.AddHttpClient<IAzureOpenAIConfig, AzureOpenAIConfig>();
        services.Configure<AzureOpenAIConfig>(context.Configuration.GetSection("AzureOpenAI"));
        services.AddHttpClient<IMCPAPIAgentConfig, MCPAPIAgentConfig>();
        services.AddHttpClient<IAzureOpenAIService, AzureOpenAIService>();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger("StartupLogger");
       

        var openAIConfig = context.Configuration.GetSection("AzureOpenAI").Get<AzureOpenAIConfig>();
        if (string.IsNullOrWhiteSpace(openAIConfig?.Endpoint) ||
            string.IsNullOrWhiteSpace(openAIConfig?.Key) ||
            string.IsNullOrWhiteSpace(openAIConfig?.DeploymentName))
        {
            var data = JsonConvert.SerializeObject(context.Configuration);
            logger.LogInformation("Configuration Data  at {time}", DateTime.UtcNow);
            logger.LogInformation(data);
            throw new InvalidOperationException("Configurations are missing in AzureOpenAI section.");
        }
    })
    .Build();

host.Run();
