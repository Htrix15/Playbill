using TelegramBot.Params;

namespace TelegramBot.Handlers.Actions.Common;

public interface IActionMessage
{
    public string Command { get; }
    public Task CreateMessages(BaseParams @params);
}
