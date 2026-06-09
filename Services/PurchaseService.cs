using Microsoft.EntityFrameworkCore;
using Tst_bot.Database;
using Tst_bot.Models;

namespace Tst_bot.Services;

public class PurchaseService
{
    private readonly AppDbcontext _context;

    public PurchaseService(AppDbcontext context)
    {
        _context = context;
    }

    public async Task<string> BuyProductAsync(long telegramId, int productId, int quantity = 1)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        if (user == null) return "Пользователь не найден. Напиши /start";

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsAvailable);
        if (product == null) return "Товар не найден или недоступен.";

        decimal totalPrice = product.Price * quantity;

        if (user.Balance < totalPrice)
            return $"Недостаточно средств. Нужно {totalPrice} руб.";

        // Покупка
        user.Balance -= totalPrice;

        var transaction = new Transaction
        {
            UserId = user.Id,
            ProductId = product.Id,
            Type = "Purchase",
            Amount = totalPrice,
            Quantity = quantity,
            Comment = $"Покупка {quantity} шт. {product.Name}"
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return $"✅ Куплено {quantity} шт. {product.Name} за {totalPrice} руб.\nНовый баланс: {user.Balance} руб.";
    }
}