using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Start : NavigationMessage
{
    public Start(MessageService messageService) : base(messageService)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/start";

    public override string MessageText => "Хай! Я бот который ищет афишу для Воронежа." + "\nРекомендую в начале поставить ограничения в настройках поиска, иначе событий может быть слишком много.";

    public override InlineKeyboardMarkup Buttons => MarkupHelper.GetStartButtons();
}
