using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class MessageBase : IActionMessage, ICommand
{
    public abstract string Command { get; }
    public static string GetCommand() => throw new NotImplementedException();

    protected readonly MessageService _messageService;
    public MessageBase(MessageService messageService)
    {
        _messageService = messageService;
    }

    public abstract Task CreateMessages(Update update);
    public virtual void InsertTo(Dictionary<string, IActionMessage> dictionary)
    {
        dictionary.Add(Command, this);
    }
}
