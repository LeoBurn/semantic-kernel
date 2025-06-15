using System.ComponentModel.DataAnnotations;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using semantic_kernel.Configuration;

Console.WriteLine("[AppConfig] Loading the application configuration...");

// Bind configuration to AppConfig class
var appConfig = LoadAppConfig();




var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    deploymentName: appConfig.OpenAi.DeploymentName,
    endpoint: appConfig.OpenAi.ApiEndpoint,
    apiKey: appConfig.OpenAi.ApiKey
);

var kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory();
chatHistory.AddSystemMessage("You're a helpful assistant");

while (true) 
{
    Console.Write("User: ");
    chatHistory.AddUserMessage(Console.ReadLine()!);
    
    var response = await chatService.GetChatMessageContentsAsync(chatHistory);
    Console.WriteLine($"Assistant: {response[^1].Content}");
    
    chatHistory.AddAssistantMessage(response[^1].Content!);
}


static AppConfig LoadAppConfig()
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    // Bind configuration to AppConfig class
    var appConfig = configuration.Get<AppConfig>();
    
    if (appConfig == null)
    {
        throw new Exception("Failed to load application configuration.");
    }
    //TODO: validate the configuration
    return appConfig;
}