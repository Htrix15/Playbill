using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Configurations;

namespace TelegramBot.Services;

public class MessageService
{
    private readonly ITelegramBotClient _botClient;
    public static readonly string[] _specialCharacters = new[] { "_", "*", "[", "]", "(", ")", "~", "`", "<", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
    private readonly BotConfiguration _botConfiguration;

    public MessageService(
      ITelegramBotClient botClient,
      IOptions<BotConfiguration> botConfiguration)
    {
        _botClient = botClient;
        _botConfiguration = botConfiguration.Value;
    }

    public async Task SendMessageAsync(ChatId chatId, string text, InlineKeyboardMarkup replyMarkup = null, bool dirtyText = true)
    {
        try
        {
            if (replyMarkup == null || !replyMarkup.InlineKeyboard.Any())
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: dirtyText ? TextNormalization(text) : text,
                    parseMode: ParseMode.MarkdownV2,
                    disableNotification: true
                );
            }
            else if (replyMarkup.InlineKeyboard.Count() <= _botConfiguration.MaxButtonsCount)
            {
                await _botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: dirtyText ? TextNormalization(text) : text,
                   parseMode: ParseMode.MarkdownV2,
                   replyMarkup: replyMarkup,
                   disableNotification: true
               );
            } 
            else
            {
                var messageCount = 1;
                var skip = 0;
                var buttons = replyMarkup.InlineKeyboard;
                var buttonsCount = 0;
                do
                {
                    var buttonsSet = replyMarkup.InlineKeyboard.Skip(skip).Take(_botConfiguration.MaxButtonsCount);
                    buttonsCount = buttonsSet.Count();
                    if (buttonsCount == 0) { break; }
                    var partLabel = $" (часть {messageCount})";
                    await _botClient.SendTextMessageAsync(
                      chatId: chatId,
                      text: dirtyText ? TextNormalization(text + partLabel) : text + partLabel,
                      parseMode: ParseMode.MarkdownV2,
                      replyMarkup: new InlineKeyboardMarkup(buttonsSet),
                      disableNotification: true
                    );
                    messageCount++;
                    skip += _botConfiguration.MaxButtonsCount;
                } while (buttonsCount == _botConfiguration.MaxButtonsCount);
            }
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
