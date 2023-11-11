using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Extensions;

public static class InlineKeyboardButtonArrayListExtension
{
    public static InlineKeyboardMarkup ToInlineKeyboardMarkup(this List<InlineKeyboardButton[]> items) => new InlineKeyboardMarkup(items);
}
