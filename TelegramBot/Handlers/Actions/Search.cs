using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Search : NavigationMessage
{
    public Search(MessageService messageService) : base(messageService)
    {
    }

    public override string Command => Commands.Search;

    public override string MessageText => "За какой период искать?";

    public override InlineKeyboardMarkup Buttons => new InlineKeyboardMarkup(new[]
        {
            new[]{ InlineKeyboardButton.WithCallbackData("На этой неделе", Commands.ThisWeek) },
            new[]{ InlineKeyboardButton.WithCallbackData("На следующей неделе", Commands.NextWeek), },
            new[]{ InlineKeyboardButton.WithCallbackData("Ближайшие 30 дней", Commands.Next30Days), },
            new[]{ InlineKeyboardButton.WithCallbackData("До конца месяца", Commands.ThisMonth), },
            new[]{ InlineKeyboardButton.WithCallbackData("До конца года", Commands.ThisYear), },
            MarkupHelper.Settings
        });

}
