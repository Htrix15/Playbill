using Models.Events;
using Telegram.Bot.Types.ReplyMarkups;
using static TelegramBot.Params.UserSettingsParams;

namespace TelegramBot.Services;

public class MarkupService
{
    private const char include = '✅';
    private const char exclude = '❌';

    private readonly InlineKeyboardButton[] _startSearch = new[] { InlineKeyboardButton.WithCallbackData("Найти события 🔍", Commands.Search) };
    private readonly InlineKeyboardButton[] _settings = new[] { InlineKeyboardButton.WithCallbackData("Настроить поиск ⚙", Commands.Settings) };

    public InlineKeyboardMarkup GetStartButtons()
    {
        return new InlineKeyboardMarkup(new[]
        {
            _startSearch,
            _settings
        });
    }

    public InlineKeyboardMarkup GetSearchDatePeriodsButtons()
    {

        return new InlineKeyboardMarkup(new[]
        {
            new[]{ InlineKeyboardButton.WithCallbackData("На этой неделе", Commands.ThisWeek) },
            new[]{ InlineKeyboardButton.WithCallbackData("На следующей неделе", Commands.NextWeek), },
            new[]{ InlineKeyboardButton.WithCallbackData("Ближайшие 30 дней", Commands.Next30Days), },
            new[]{ InlineKeyboardButton.WithCallbackData("До конца месяца", Commands.ThisMonth), },
            new[]{ InlineKeyboardButton.WithCallbackData("До конца года", Commands.ThisYear), },
            _settings
        });
    }

    public InlineKeyboardMarkup GetSettingsButtons()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]{ InlineKeyboardButton.WithCallbackData("Где искать", Commands.Billboards) },
            new[]{ InlineKeyboardButton.WithCallbackData("Какие события искать", Commands.EventTypes), },
            new[]{ InlineKeyboardButton.WithCallbackData("Какие дни недели искать", Commands.DaysOfWeek), },
            //new[]{ InlineKeyboardButton.WithCallbackData("На каких площадках искать", Commands.Places), },
            new[]{ InlineKeyboardButton.WithCallbackData("Добавить ли праздничные выходные", Commands.AddHolidays), },
             _startSearch,
        });
    }

    public InlineKeyboardMarkup GetSettingsButtons(List<Setting> settings, string key)
    {
        var inlineKeyboardButton = new List<InlineKeyboardButton[]>();
        settings.ForEach(setting => {
            var excludeFlag = setting.Exclude ? 1 : 0;
            var excludeLabel = setting.Exclude ? exclude : include;
            inlineKeyboardButton.Add(new[] { InlineKeyboardButton.WithCallbackData($"{setting.Label} {excludeLabel}", key + setting.Id + $"_{excludeFlag}") });
        });

        inlineKeyboardButton.AddRange(
        new[] {
            _startSearch,
            new[]{ InlineKeyboardButton.WithCallbackData("Вернуться к списку настроек ⤴︎", Commands.Settings) },
        });
        return new InlineKeyboardMarkup(inlineKeyboardButton);
    }

    public InlineKeyboardMarkup GetEventButtons(List<EventLink> eventLinks)
    {
        return new InlineKeyboardMarkup(new[]
        {
            eventLinks.Select(eventLink =>  InlineKeyboardButton.WithUrl(eventLink.BillboardType.ToString(), eventLink.Path))
        });
    }

    public InlineKeyboardMarkup ReplaceButtons(InlineKeyboardMarkup inlineKeyboardMarkup, string key)
    {
        var replyKeyboardMarkup = inlineKeyboardMarkup;

        var updatedButton = replyKeyboardMarkup.InlineKeyboard.First(buttons => buttons.Any(button => button.CallbackData == key));

        var text = updatedButton.First().Text;
        updatedButton.First().Text = text.Contains(include) ? text.Replace(include, exclude) : text.Replace(exclude, include);

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
