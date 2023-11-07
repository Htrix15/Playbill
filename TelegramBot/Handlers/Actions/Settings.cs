using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Settings : NavigationMessage
{
    public Settings(MessageService messageService) : base(messageService)
    {
    }

    public override string Command => Commands.Settings;

    public override string MessageText => "Что настроить?";

    public override InlineKeyboardMarkup Buttons => new InlineKeyboardMarkup(new[]
    {
        new[]{ InlineKeyboardButton.WithCallbackData("Где искать", Commands.Billboards) },
        new[]{ InlineKeyboardButton.WithCallbackData("Какие события искать", Commands.EventTypes), },
        new[]{ InlineKeyboardButton.WithCallbackData("Какие дни недели искать", Commands.DaysOfWeek), },
        new[]{ InlineKeyboardButton.WithCallbackData("Добавить ли праздничные выходные", Commands.AddHolidays), },
        MarkupHelper.StartSearch,
    });

}
