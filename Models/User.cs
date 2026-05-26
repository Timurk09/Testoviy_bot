using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tst_bot.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public long TelegramId { get; set; }
    
    [MaxLength(50)]
    public string? Username { get; set; } //Ник, который типа @nickname(может быть null)
    
    [MaxLength(100)]
    public string? FirstName { get; set; } = String.Empty; 
    
    [Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; } = 0;

    public DateTime LastMoneyIssued { get; set; } = DateTime.UtcNow; //время последней выдачи денег
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>(); //список транзакций пользователя
}