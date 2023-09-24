using Telegram.Bot.Polling;
using Telegram.Bot;

namespace TelegramBot.Services;

public class ReceiverService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly ReceiverOptions _receiverOptions;

    public ReceiverService(
        ITelegramBotClient botClient,
        IUpdateHandler updateHandler,
        ReceiverOptions receiverOptions)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _receiverOptions = receiverOptions;
    }
    public async Task ReceiveAsync()
    {
        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username}");

        await _botClient.ReceiveAsync(
            updateHandler: _updateHandler,
            receiverOptions: _receiverOptions);
    }
}
