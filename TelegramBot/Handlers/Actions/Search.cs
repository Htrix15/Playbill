using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Search : MessageBase
{
    public Search(MessageService messageService) : base(messageService)
    {
    }

    public override string Command => Commands.Search;

    public override Task CreateMessages(BaseParams @params) => _messageService.GetSearchDatePeriodsMessageAsync(@params as MessageParams);
}
