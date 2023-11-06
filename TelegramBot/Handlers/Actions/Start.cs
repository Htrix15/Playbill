using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Start : MessageBase
{
    public Start(MessageService messageService) : base(messageService)
    {
    }

    public override string Command => Commands.Start;

    public override Task CreateMessages(BaseParams @params) => _messageService.GetStartMessageAsync(@params as MessageParams);
}
