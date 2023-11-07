using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Params;

namespace TelegramBot.Extensions;

public static class UpdateExtension
{
    public static ChatId GetChatId(this Update update) => update.Type switch
    {
        UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat.Id,
        UpdateType.Message => update.Message.Chat.Id,
        _ => throw new ArgumentOutOfRangeException(nameof(update.Type), $"Not expected direction value: {update.Type}"),
    };

    public static string GetKey(this Update update) => update.Type switch {
        UpdateType.CallbackQuery when update.CallbackQuery.Data.Contains('_') => update.CallbackQuery.Data.ToLower().Split('_')[0].ToLower(),
        UpdateType.CallbackQuery => update.CallbackQuery.Data.ToLower(),
        UpdateType.Message => update.Message.Text.ToLower(),
        _ => throw new ArgumentOutOfRangeException(nameof(update.Type), $"Not expected direction value: {update.Type}"),
    };

    public static GetEventsParams GetGetEventsParams(this Update update, DatePeriods datePeriod)
    {
        return new GetEventsParams()
        {
            ChatId = update.CallbackQuery.Message.Chat.Id,
            UserId = update.CallbackQuery.From.Id,
            DatePeriod = datePeriod
        };
    }

    public static UserSettingsParams GetUserSettingsParams(this Update update)
    {
        return new UserSettingsParams()
        {
            ChatId = update.CallbackQuery.Message.Chat.Id,
            UserId = update.CallbackQuery.From.Id,
        };
    }

    public static UpdateUserSettingsParams GetUpdateUserSettingsParams(this Update update)
    {
        var callbackQuery = update.CallbackQuery;
        var keys = callbackQuery.Data.ToLower().Split('_');
        var entityId = keys[1];
        var oldExclude = keys[2] == "1";

        return new UpdateUserSettingsParams()
        {
            ChatId = callbackQuery.Message.Chat.Id,
            UserId = update.CallbackQuery.From.Id,
            MessageId = callbackQuery.Message.MessageId,
            Key = callbackQuery.Data,
            EntityId = entityId,
            Exclude = !oldExclude,
            Markup = callbackQuery.Message.ReplyMarkup
        };
        
    }
}
