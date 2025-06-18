using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using semantic_kernel.Configuration;
using semantic_kernel.Plugin;

Console.WriteLine("[Program] Loading the application configuration...");

// Bind configuration to AppConfig class
var appConfig = LoadAppConfig();



//Create the Kernel builder and add Azure OpenAI Chat Completion service
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    deploymentName: appConfig.OpenAi.DeploymentName,
    endpoint: appConfig.OpenAi.ApiEndpoint,
    apiKey: appConfig.OpenAi.ApiKey
);
var kernel = builder.Build();

// Register plugins
Console.WriteLine("[Program] Registering DynamicQuery plugin...");


//kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new RunTimeQueryPlugin(), "RunTimeQuery"));
kernel.Plugins.AddFromType<CompanyPlugin>("Company");
kernel.Plugins.AddFromType<WorkerPlugin>("Worker");
kernel.Plugins.AddFromType<FormPlugin>("Form");
kernel.Plugins.AddFromType<MusicPlugin>("Music");
OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
};
var chatService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
while (true) 
{
    Console.Write("User: ");
    var input = Console.ReadLine();
    history.AddUserMessage(input);
    var result = await chatService.GetChatMessageContentsAsync(
        history,
        executionSettings: openAiPromptExecutionSettings,
        kernel: kernel);
    Console.WriteLine($"AI: {result[^1].Content}");
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

