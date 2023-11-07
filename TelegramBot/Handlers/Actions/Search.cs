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
    public override string Command => GetCommand();
    public static new string GetCommand() => "/search";

    public override string MessageText => "За какой период искать?";

    public override InlineKeyboardMarkup Buttons => new InlineKeyboardMarkup(new[]
    {
        new[]{ InlineKeyboardButton.WithCallbackData("На этой неделе", ThisWeekEvents.GetCommand()) },
        new[]{ InlineKeyboardButton.WithCallbackData("На следующей неделе", NextWeekEvents.GetCommand()) },
        new[]{ InlineKeyboardButton.WithCallbackData("Ближайшие 30 дней", Next30DaysEvents.GetCommand()) },
        new[]{ InlineKeyboardButton.WithCallbackData("До конца месяца", ThisMonthEvents.GetCommand()) },
        new[]{ InlineKeyboardButton.WithCallbackData("До конца года", ThisYearEvents.GetCommand()) },
        MarkupHelper.Settings
    });

}
