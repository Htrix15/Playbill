using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Settings : NavigationMessage
{
    public Settings(MessageService messageService) : base(messageService)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/settings";

    public override string MessageText => "Что настроить?";

    public override InlineKeyboardMarkup Buttons => new InlineKeyboardMarkup(new[]
    {
        new[]{ InlineKeyboardButton.WithCallbackData("Где искать", UserBillboards.GetCommand()) },
        new[]{ InlineKeyboardButton.WithCallbackData("Какие события искать", UserEventTypes.GetCommand()) },
        new[]{ InlineKeyboardButton.WithCallbackData("Какие дни недели искать", UserDaysOfWeek.GetCommand()) },
        new[]{ InlineKeyboardButton.WithCallbackData("Добавить ли праздничные выходные", UserAddHolidays.GetCommand()) },
        MarkupHelper.StartSearch,
    });

}
