using Telegram.Bot.Types;

namespace TelegramBot.Handlers.Actions.Common;

public interface IActionMessage
{
    public string Command { get; }
    public Task CreateMessages(Update update);
}
