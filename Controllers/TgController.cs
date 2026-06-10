using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Tst_bot.Services;
namespace Tst_bot.Controllers;

[ApiController]
[Route("api/bot")]
public class TgController:ControllerBase
{
    private readonly ITelegramBotClient  _botClient;
    private readonly UserService _userService;
    private readonly ProductService _productService;
    private readonly BalanceService _balanceService;
    private readonly PurchaseService _purchaseService;

    public TgController(ITelegramBotClient botClient, UserService userService, ProductService productService, BalanceService balanceService, PurchaseService purchaseService)
    {
        _botClient = botClient;
        _userService = userService;
        _productService = productService;
        _balanceService = balanceService;
        _purchaseService = purchaseService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        if (update.Message == null) return Ok();
        var message = update.Message;
        var chatId = message.Chat.Id;
        var text = message.Text?.Trim().ToLower() ?? "";
        try
        {
            await (text switch
            {
                "/start" => HandleStart(chatId, message.From),
                "/balance" => HandleBalance(chatId),
                "/shop" => HandleShop(chatId),
                "/income" => HandleIncome(chatId),
                string t when t.StartsWith("/buy") => HandleBuy(chatId, message.Text),

                _ => _botClient.SendMessage(chatId,
                    "Неизвестная команда.\n\n" +
                    "Доступные команды:\n/start — начать\n/balance — баланс\n/shop — магазин")
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await _botClient.SendMessage(chatId, "Ошибка сервера, попробуй позже");
        }
        return Ok();
    }

    private async Task HandleStart(long chatId, User? from)
    {
        var user = await _userService.GetOrCreateUserAsync(chatId, from?.FirstName, from?.Username);
        await _botClient.SendMessage(chatId,
            $"Привет, {user.FirstName}!\n\n" +
            $"Баланс: **{user.Balance}** руб\n\n" +
            "Команды: /balance | /shop");
    }

    private async Task HandleBalance(long chatId)
    {
        var user = await _userService.GetUserAsync(chatId);
        if (user == null)
        {
            await _botClient.SendMessage(chatId, "сначала напиши /start");
            return;
        }
        await _botClient.SendMessage(chatId,
            $"Твой баланс: **{user.Balance}** руб",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
    }
    
    private  async Task HandleShop(long chatId)
    {
        var products = await _productService.GetAvaibleProductsAsync();
        if (products.Count == 0) { await _botClient.SendMessage(chatId, "Магазин пока пустой"); return; }
        var sb = "🛒 **Магазин**\n\n";
        foreach (var p in products)
        {
            sb += $"• {p.Name} — **{p.Price}** руб.\n   {p.Description}\n\n";
        }
        await _botClient.SendMessage(chatId, sb);
    }

    private async Task HandleIncome(long chatId)
    {
        var result = await _balanceService.IssueDailyMoneyAsync(chatId);
        if (result == -1)
        {
            await _botClient.SendMessage(chatId, "Деньги уже выдавали недавно. Следующая выдача через пару часов.");
        }
        else
        {
            await _botClient.SendMessage(chatId, $"✅ Выдано **{result}** руб. на баланс!");
        }
    }

    private async Task HandleBuy(long chatId, string text)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[1], out int productId))
        {
            await _botClient.SendMessage(chatId, "❌ Использование: `/buy <id_товара> [количество]`\nПример: `/buy 1 3`", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            return;
        }
        int quantity = 1;
        if (parts.Length >= 3 && int.TryParse(parts[2], out int parsedQuantity))
        {
            quantity = parsedQuantity;
        }
        if (quantity <= 0)
        {
            await _botClient.SendMessage(chatId, "❌ Количество товара должно быть больше 0!");
            return;
        }
        try
        {
            var result = await _purchaseService.BuyProductAsync(chatId, productId, quantity);
            await _botClient.SendMessage(chatId, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при покупке: {ex}");
            await _botClient.SendMessage(chatId, "❌ Произошла ошибка на сервере при оформлении покупки.");
        }
    }
}