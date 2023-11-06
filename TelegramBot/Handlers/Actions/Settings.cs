using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Settings : MessageBase
{
    public Settings(MessageService messageService) : base(messageService)
    {
    }

    public override string Command => Commands.Settings;

    public override Task CreateMessages(BaseParams @params) => _messageService.GetSettingsMessageAsync(@params as MessageParams);
}
