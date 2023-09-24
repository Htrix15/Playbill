using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Params;

namespace TelegramBot.Services;

public class MessageService
{
    private readonly ITelegramBotClient _botClient;
    private readonly MarkupService _markupService;
    private readonly string[] _specialCharacters = new[] { "_", "*", "[", "]", "(", ")", "~", "`", "<", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };

    public MessageService(
      ITelegramBotClient botClient,
      MarkupService markupService)
    {
        _botClient = botClient;
        _markupService = markupService;
    }
    public async Task NotFoundCommandMessageAsync(MessageParams messageParams)
    {
        await _botClient.SendTextMessageAsync(
            chatId: messageParams.ChatId,
            text: TextNormalization("Бот не знает такой команды :("),
            parseMode: ParseMode.MarkdownV2
            );
    }

    public async Task GetStartMessageAsync(MessageParams messageParams)
    {
        await _botClient.SendTextMessageAsync(
            chatId: messageParams.ChatId,
            text: TextNormalization("Хай! Я бот который ищет афишу для Воронежа." + "\nРекомендую в начале поставить ограничения в настройках поиска, иначе событий может быть слишком много."),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: _markupService.GetStartButtons()
            );
    }

    public async Task GetSearchDatePeriodsMessageAsync(MessageParams messageParams)
    {
        await _botClient.SendTextMessageAsync(
            chatId: messageParams.ChatId,
            text: TextNormalization("За какой период искать?"),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: _markupService.GetSearchDatePeriodsButtons()
            );
    }

    public async Task GetSettingsMessageAsync(MessageParams messageParams)
    {
        await _botClient.SendTextMessageAsync(
            chatId: messageParams.ChatId,
            text: TextNormalization("Что настроить?"),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: _markupService.GetSettingsButtons()
            );
    }

    public async Task GetSettingsListMessageAsync(UserSettingsParams messageParams, string key)
    {
        await _botClient.SendTextMessageAsync(
            chatId: messageParams.ChatId,
            text: TextNormalization("Нажмите на то, что хотите добавить или удалить"),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: _markupService.GetSettingsButtons(messageParams.Settings, key)
            );
    }

    public async Task UpdateSettingsListMessageAsync(UpdateUserSettingsParams messageParams)
    {
        await _botClient.EditMessageReplyMarkupAsync(
            messageParams.ChatId,
            messageParams.MessageId,
            _markupService.ReplaceButtons(messageParams.Markup, messageParams.Key));
    }

    public async Task GetStartSearchMessageAsync(MessageParams messageParams)
    {
        await _botClient.SendTextMessageAsync(
            chatId: messageParams.ChatId,
            text: TextNormalization("Начат поиск событий..."),
            parseMode: ParseMode.MarkdownV2,
            disableNotification: true
            );
    }

    public async Task GetEventMessagesAsync(EventsMessagesParams eventsMessagesParams)
    {
        foreach (var @event in eventsMessagesParams.Events)
        {
            var caption = GetCaptionFromEvent(@event);
            var replyMarkup = _markupService.GetEventButtons(@event.Links);
            try
            {
                await _botClient.SendPhotoAsync(
                    chatId: eventsMessagesParams.ChatId,
                    photo: InputFile.FromUri(@event.ImagePath),
                    caption: caption,
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: replyMarkup,
                    disableNotification: true
                );
            }
            catch 
            {
                await _botClient.SendTextMessageAsync(
                    chatId: eventsMessagesParams.ChatId,
                    text: caption,
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: replyMarkup,
                    disableNotification: true
                );
            }
        }

        await _botClient.SendTextMessageAsync(
            chatId: eventsMessagesParams.ChatId,
            text: TextNormalization("Поиск завершен!"),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: _markupService.GetStartButtons()
            );
    }

    private string GetCaptionFromEvent(Models.Events.Event @event)
    {
        var dateMask = @event.Date!.Value.Hour != 0 ? "dd MMMM HH:mm (dddd)" : "dd MMMM (dddd)";
        var times = @event.Sessions?.Any() ?? false
            ? $"({string.Join(", ", @event.Sessions.Select(session => session.ToString("HH:mm")))})"
            : "";
        return $"*{TextNormalization(@event.Title)}*" +
            $"\n{@event.Type}" +
            $"\n*{TextNormalization(@event.Date.Value.ToString(dateMask))}*" +
            $"\n{TextNormalization(times)}" +
            $"\n{TextNormalization(@event.Place)}";
    }

  
    private string TextNormalization(string text)
    {
        foreach (var specialCharacter in _specialCharacters)
        {
            text = text.Replace(specialCharacter, $"\\{specialCharacter}");
        }

        return text;
    }
}
