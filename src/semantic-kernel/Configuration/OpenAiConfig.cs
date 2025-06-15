using System.ComponentModel.DataAnnotations;

namespace semantic_kernel.Configuration;

public class OpenAiConfig
{
    [Required]
    public string ApiKey { get; set; }
    [Required]
    [Url]
    public string ApiEndpoint { get; set; }
    [Required]
    public string DeploymentName { get; set; }
}