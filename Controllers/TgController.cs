using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Tst_bot.Database;
using Tst_bot.Models;

namespace Tst_bot.Controllers;

[ApiController]
[Route("api/bot")]
public class TgController:ControllerBase
{
    readonly AppDbcontext _context;
    readonly ITelegramBotClient _botClient;

    public TgController(AppDbcontext context, ITelegramBotClient botClient)
    {
        _context = context;
        _botClient = botClient;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        if (update.Message == null) return Ok();

        var message = update.Message;
        var chatId = message.Chat.Id;
        var text = message.Text?.ToLower() ?? "";
        
        if (text == "/start")
        {
            await _botClient.SendMessage(chatId, "Привет! Бот запущен через webhook.");
        }
        else
        {
            await _botClient.SendMessage(chatId, $"Ты написал: {message.Text}");
        }
        return Ok();
    }
}