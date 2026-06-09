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

    public TgController(ITelegramBotClient botClient, UserService userService, ProductService productService, BalanceService balanceService)
    {
        _botClient = botClient;
        _userService = userService;
        _productService = productService;
        _balanceService = balanceService;
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
}