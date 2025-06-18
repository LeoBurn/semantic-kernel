using System.ComponentModel.DataAnnotations;

namespace semantic_kernel.Data;

public class Company
{
    public int Id { get; set; }
    [Display(Description = "name of the company")]
    public string Name { get; set; }
}