using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Params;

public class UpdateUserSettingsParams : BaseParams
{
    public long UserId { get; set; }
    public int MessageId { get; set; }
    public string Key { get; set; }
    public string EntityId { get; set; }
    public bool Exclude { get; set; }
    public InlineKeyboardMarkup Markup { get; set; }
}
