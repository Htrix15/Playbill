using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class MessageBase : IActionMessage
{
    public abstract string Command { get; }
    protected readonly MessageService _messageService;
    public MessageBase(MessageService messageService)
    {
        _messageService = messageService;
    }
    public abstract Task CreateMessages(BaseParams @params);
    public virtual void InsertTo(Dictionary<string, IActionMessage> dictionary)
    {
        dictionary.Add(Command, this);
    }
}
