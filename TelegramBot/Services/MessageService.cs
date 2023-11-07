using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Services;

public class MessageService
{
    private readonly ITelegramBotClient _botClient;
    public static readonly string[] _specialCharacters = new[] { "_", "*", "[", "]", "(", ")", "~", "`", "<", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };

    public MessageService(
      ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendMessageAsync(ChatId chatId, string text, InlineKeyboardMarkup replyMarkup = null, bool dirtyText = true)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: dirtyText ? TextNormalization(text) : text,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: replyMarkup,
                disableNotification: true
            );
        }
        catch
        {

        }
    }

    public async Task SendMessageWithPhotoAsync(ChatId chatId, 
        string text, 
        string imagePath,
        InlineKeyboardMarkup replyMarkup = null, 
        bool dirtyText = true)
    {
        try
        {
            await _botClient.SendPhotoAsync(
                chatId: chatId,
                photo: InputFile.FromUri(imagePath),
                caption: dirtyText ? TextNormalization(text) : text,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: replyMarkup,
                disableNotification: true);
        }
        catch
        {
            await _botClient.SendTextMessageAsync(
               chatId: chatId,
               text: dirtyText ? TextNormalization(text) : text,
               parseMode: ParseMode.MarkdownV2,
               replyMarkup: replyMarkup,
               disableNotification: true
           );
        }
    }

    public async Task EditMessageAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup)
    {
        try
        {
            await _botClient.EditMessageReplyMarkupAsync(
                chatId,
                messageId,
                replyMarkup);
        }
        catch
        {

        }
    }

 
    public static string TextNormalization(string text)
    {
        foreach (var specialCharacter in _specialCharacters)
        {
            text = text.Replace(specialCharacter, $"\\{specialCharacter}");
        }

        return text;
    }
}
