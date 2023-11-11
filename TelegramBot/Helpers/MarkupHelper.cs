using Models.Events;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Handlers.Actions;
using static TelegramBot.Params.UserSettingsParams;

namespace TelegramBot.Helpers;

public static class MarkupHelper
{
    public static readonly char Include = '✅';
    public static readonly char Exclude = '❌';

    public static readonly InlineKeyboardButton[] StartSearch = new[] { InlineKeyboardButton.WithCallbackData("Найти события 🔍", Search.GetCommand()) };
    public static readonly InlineKeyboardButton[] Settings = new[] { InlineKeyboardButton.WithCallbackData("Настроить поиск ⚙", Handlers.Actions.Settings.GetCommand()) };
    public static readonly InlineKeyboardButton[] BackToSettings = new[] { InlineKeyboardButton.WithCallbackData("Вернуться к списку настроек ⤴︎", Handlers.Actions.Settings.GetCommand()) };
    public static readonly List<InlineKeyboardButton[]> SettingsSet = new List<InlineKeyboardButton[]> { StartSearch, BackToSettings };
    public static InlineKeyboardMarkup GetStartButtons()
    {
        return new InlineKeyboardMarkup(new[]
        {
            StartSearch,
            Settings
        });
    }

    public static List<InlineKeyboardButton[]> CreateToogleButtons(List<Setting> settings, string key)
    {
        var inlineKeyboardButton = new List<InlineKeyboardButton[]>();
        settings.ForEach(setting =>
        {
            var excludeFlag = setting.Exclude ? 1 : 0;
            var excludeLabel = setting.Exclude ? Exclude : Include;
            inlineKeyboardButton.Add(new[] { InlineKeyboardButton.WithCallbackData($"{setting.Label} {excludeLabel}", key + setting.Id + $"_{excludeFlag}") });
        });
        return inlineKeyboardButton;
    }

    public static InlineKeyboardMarkup GetEventButtons(List<EventLink> eventLinks)
    {
        return new InlineKeyboardMarkup(new[]
        {
            eventLinks.Select(eventLink =>  InlineKeyboardButton.WithUrl(eventLink.BillboardType.ToString(), eventLink.Path))
        });
    }

    public static InlineKeyboardMarkup ReplaceButtons(InlineKeyboardMarkup inlineKeyboardMarkup, string key)
    {
        var replyKeyboardMarkup = inlineKeyboardMarkup;

        var updatedButton = replyKeyboardMarkup.InlineKeyboard.First(buttons => buttons.Any(button => button.CallbackData == key));

        var text = updatedButton.First().Text;
        updatedButton.First().Text = text.Contains(Include) ? text.Replace(Include, Exclude) : text.Replace(Exclude, Include);

        var keys = key.Split("_");
        if (keys.Last() == "0")
        {
            keys[keys.Length - 1] = "1";
        }
        else
        {
            keys[keys.Length - 1] = "0";
        }
        updatedButton.First().CallbackData = string.Join("_", keys);
        return replyKeyboardMarkup;
    }
}
