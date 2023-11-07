using Models.Search;
using Models.Users;
using TelegramBot.Params;
using TelegramBot.Services;
using static TelegramBot.Params.UserSettingsParams;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class SettingsMessageBase : MessageBase
{
    protected readonly SearchOptions _searchOptions;
    protected readonly IUserSettingsRepository _userSettingsRepository;
    protected string CommandKey => $"{Command.TrimStart('/')}_";
    protected string CollbackKey => Command.TrimStart('/');

    public string MessageText => "Нажмите на то, что хотите добавить или удалить";


    protected SettingsMessageBase CollbackAction;

    public SettingsMessageBase(MessageService messageService, 
        SearchOptions searchOptions,
        IUserSettingsRepository userSettingsRepository) : base(messageService)
    {
        _searchOptions = searchOptions;
        _userSettingsRepository = userSettingsRepository;
    }

    protected void SetExclude<T>(IEnumerable<T> excludeCollection, List<Setting> result)
    {
        excludeCollection.ToList().ForEach(excludeBillboard =>
        {
            var billboard = result.FirstOrDefault(billboard => billboard.Id == Convert.ToInt32(excludeBillboard));
            if (billboard is not null)
            {
                billboard.Exclude = true;
            }
        });
    }

    protected Func<T, Setting> ConverFunctions<T>() => billboard => new Setting()
    {
        Exclude = false,
        Id = Convert.ToInt32(billboard),
        Label = billboard.ToString()
    };

    protected async Task CreateMessages(UserSettingsParams @params, List<Setting> settings)
    {
        await _messageService.SendMessageAsync(@params.ChatId, MessageText, MarkupHelper.GetSettingsButtons(settings, CommandKey));
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
