using System.ComponentModel.DataAnnotations;

namespace semantic_kernel.Data;

public class Form
{
    public int Id { get; set; }
    [Display(Description = "ID of the company that owns the form Foreign Key")]
    public int CompanyId { get; set; }
    [Display(Description = "ID of the worker who created the form Foreign Key")]
    public int WorkerId { get; set; }
    
    [Display(Description = "type of the form (e.g., internal, public, private)")]
    public string Type { get; set; }
    [Display(Description = "name of the form")]
    public string Name { get; set; }
}