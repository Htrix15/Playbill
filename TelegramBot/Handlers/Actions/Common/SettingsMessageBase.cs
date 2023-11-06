using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class SettingsMessageBase : MessageBase
{
    protected readonly UserSettingsService _userSettingsService;
    protected string CommandKey => $"{Command.TrimStart('/')}_";
    protected string CollbackKey => Command.TrimStart('/');

    protected SettingsMessageBase CollbackAction;

    public SettingsMessageBase(MessageService messageService, UserSettingsService userSettingsService) : base(messageService)
    {
        _userSettingsService = userSettingsService;
    }
    protected async Task CreateMessages(UserSettingsParams @params, List<UserSettingsParams.Setting> settings)
    {
        @params.Settings = settings;
        await _messageService.GetSettingsListMessageAsync(@params, CommandKey);
    }
    public override void InsertTo(Dictionary<string, IActionMessage> dictionary)
    {
        dictionary.Add(Command, this);
        if (CollbackAction != null)
        {
            dictionary.Add(CollbackKey, CollbackAction);
        }
    }
}
