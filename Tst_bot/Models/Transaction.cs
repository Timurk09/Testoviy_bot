using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tst_bot.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }

    public User User { get; set; } = null!;
    
    //покупка товара или начисление денег
    public int? ProductId { get; set; } //null - начисление денег
    public Product? Product { get; set; }

    [MaxLength(50)] public string Type { get; set; } = "Purchase"; // "Purchase" или "Income"
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public int Quantity { get; set; } = 1;
    
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    [MaxLength(200)]
    public string? Comment { get; set; }
}