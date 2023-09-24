
namespace TelegramBot.Params;

public class UserSettingsParams : BaseParams
{
    public required long UserId { get; set; }
    public List<Setting> Settings { get; set; }

    public class Setting
    {
        public string Label { get; set; }
        public int Id { get; set; }
        public bool Exclude { get; set; }
    }
}
