using System.ComponentModel.DataAnnotations;

namespace semantic_kernel.Configuration;

public class AppConfig
{
    [Required]
    public OpenAiConfig OpenAi { get; set; }
    
}