using Microsoft.Extensions.Options;
using Models.Search;
using Models.Users;
using TelegramBot.Handlers.Actions;
using TelegramBot.Handlers.Actions.Common;

namespace TelegramBot.Services;

public class MessageActionsService
{
    private readonly MessageService _messageService;
    private readonly EventService _eventService;
    private readonly SearchOptions _searchOptions;
    private readonly IUserSettingsRepository _userSettingsRepository;

    private readonly List<MessageBase> NavigationMessage = new();
    private readonly List<SettingsMessageBase> UserSettings = new();
    private readonly List<EventMessageBase> GetEvents = new();

    public MessageActionsService(IOptions<SearchOptions> defaultOptions, 
        MessageService messageService,
        EventService eventService,
        IUserSettingsRepository userSettingsRepository)
    {
        _searchOptions = defaultOptions.Value;
        _userSettingsRepository = userSettingsRepository;
        _messageService = messageService;
        _eventService = eventService;

        NavigationMessage.Add(new Start(_messageService));
        NavigationMessage.Add(new Search(_messageService));
        NavigationMessage.Add(new Settings(_messageService));

        UserSettings.Add(new UserBillboards(_messageService, _searchOptions, _userSettingsRepository));
        UserSettings.Add(new UserEventTypes(_messageService, _searchOptions, _userSettingsRepository));
        UserSettings.Add(new UserDaysOfWeek(_messageService, _searchOptions, _userSettingsRepository));
        UserSettings.Add(new UserAddHolidays(_messageService, _searchOptions, _userSettingsRepository));

        GetEvents.Add(new ThisWeekEvents(_messageService, _eventService));
        GetEvents.Add(new NextWeekEvents(_messageService, _eventService));
        GetEvents.Add(new ThisMonthEvents(_messageService, _eventService));
        GetEvents.Add(new Next30DaysEvents(_messageService, _eventService));
        GetEvents.Add(new ThisYearEvents(_messageService, _eventService));
    }


    public Dictionary<string, IActionMessage> Get()
    {
        var messageActions = new Dictionary<string, IActionMessage>();

        NavigationMessage.ForEach(action => action.InsertTo(messageActions));
        UserSettings.ForEach(userSetting => userSetting.InsertTo(messageActions));
        GetEvents.ForEach(getEvent => getEvent.InsertTo(messageActions));
        return messageActions;
    }
}
