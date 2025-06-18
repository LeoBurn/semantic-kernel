using System.ComponentModel.DataAnnotations;

namespace semantic_kernel.Data;

public class Worker
{
    public int Id { get; set; }
    [Display(Description = "ID of the company that owns the form Foreign Key")]
    public int CompanyId { get; set; }
    
    [Display(Description = "The name of workers")]
    public string Name { get; set; }

}