using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tst_bot.Models;

public class Product
{
    [Key] 
    public int Id { get; set; }
    
    [Required, MaxLength(100)] 
    public string Name { get; set; } = String.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;
}