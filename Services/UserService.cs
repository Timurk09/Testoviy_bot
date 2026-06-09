using Microsoft.EntityFrameworkCore;
using Tst_bot.Database;
using Tst_bot.Models;

namespace Tst_bot.Services;

public class UserService
{
    private readonly AppDbcontext _context;

    public UserService(AppDbcontext context)
    {
        _context = context;
    }

    public async Task<User> GetOrCreateUserAsync(long telegramId, string? firstName, string? username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        if (user == null)
        {
            user = new User
            {
                TelegramId = telegramId,
                FirstName = firstName ?? "User",
                Username = username,
                Balance = 1000,
                LastMoneyIssued = DateTime.UtcNow
            };
        }

        return user;
    }

    public async Task<User?> GetUserAsync(long telegramId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
    }
}