using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Extensions;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class NavigationMessage : MessageBase
{
    public abstract string MessageText { get; }
    public abstract InlineKeyboardMarkup Buttons { get; }
    protected NavigationMessage(MessageService messageService) : base(messageService)
    {
    }
    public override Task CreateMessages(Update update) => _messageService.SendMessageAsync(update.GetChatId(), MessageText, Buttons);
}
