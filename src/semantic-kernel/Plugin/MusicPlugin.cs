using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace semantic_kernel.Plugin;

public class MusicPlugin
{

    public string UriPath = "https://www.vagalume.com.br/{0}/index.js";
    
    public MusicPlugin()
    {
        Console.WriteLine("[MusicPlugin] Initializing MusicPlugin...");
    }
    
    [KernelFunction("GetMusics")]
    [Description("Gets the json data from a endpoint that give all musics from a artist")]
    [return: Description("The json data from the endpoint but return top toplyrics")]
    public async Task<string> GetMusics(Kernel kernel,
        [Description("The name of the artist to search, if the artist name have two names use a dash (-) to separate them, for example: 'The Beatles' will be 'the-beatles'")]
        string artistName)
    {
        Console.WriteLine("[MusicPlugin] Run GetMusics");
        if (string.IsNullOrWhiteSpace(artistName))
            return "❌ Artist name cannot be empty.";
        
        var endpoint = string.Format(UriPath, artistName.ToLower().Replace(" ", "-"));
        
        var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(endpoint);

        return response;
    }
}