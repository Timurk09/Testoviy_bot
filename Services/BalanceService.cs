using Microsoft.EntityFrameworkCore;
using Tst_bot.Database;
using Tst_bot.Models;

namespace Tst_bot.Services;

public class BalanceService
{
    private readonly AppDbcontext _context;

    public BalanceService(AppDbcontext context)
    {
        _context = context;
    }

    public async Task<decimal> IssueDailyMoneyAsync(long telegramId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        if (user == null) return 0;
        var now = DateTime.UtcNow;
        var hoursSinceLast = (now - user.LastMoneyIssued).TotalHours;
        if (hoursSinceLast < 2)
        {
            return -1;
        }
        const decimal amount = 500;
        user.Balance += amount;
        user.LastMoneyIssued = now;
        var Transaction = new Transaction
        {
            UserId = user.Id,
            Type = "Income",
            Amount = amount,
            Comment = "ежедневная выдача"
        };
        _context.Transactions.Add(Transaction); 
        await _context.SaveChangesAsync();
        return amount;
    }
}