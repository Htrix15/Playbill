using Microsoft.Extensions.Options;
using Models;
using Models.Places;
using Models.Search;
using Models.Users;
using TelegramBot.Configurations;
using TelegramBot.Handlers.Actions;
using TelegramBot.Handlers.Actions.Common;

namespace TelegramBot.Services;

public class MessageActionsService
{
    private readonly MessageService _messageService;
    private readonly EventService _eventService;
    private readonly SearchOptions _searchOptions;
    private readonly BotConfiguration _botConfiguration;
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly IPlaceRepository _placeRepository;

    private readonly List<MessageBase> NavigationMessage = new();
    private readonly List<SettingsMessageBase<UserSettings>> UserSettings = new();
    private readonly List<SettingsMessageBase<UserSettings, Place>> UserSettingsWithPlace = new();
    private readonly List<EventMessageBase> GetEvents = new();
    private readonly List<CollbackMessage<UserSettings>> UserSettingsCollbacks = new();

    public MessageActionsService(IOptions<SearchOptions> defaultOptions,
        IOptions<BotConfiguration> botConfiguration,
        MessageService messageService,
        EventService eventService,
        IRepository<UserSettings> userSettingsRepository,
        IRepository<Place> placeRepository)
    {
        _searchOptions = defaultOptions.Value;
        _botConfiguration = botConfiguration.Value;
        _userSettingsRepository = (IUserSettingsRepository)userSettingsRepository;
        _placeRepository = (IPlaceRepository)placeRepository;
        _messageService = messageService;
        _eventService = eventService;

        NavigationMessage.Add(new Start(_messageService));
        NavigationMessage.Add(new Search(_messageService));
        NavigationMessage.Add(new Settings(_messageService));

        UserSettings.Add(new UserBillboards(_messageService, _searchOptions, _userSettingsRepository));
        UserSettings.Add(new UserEventTypes(_messageService, _searchOptions, _userSettingsRepository));
        UserSettings.Add(new UserDaysOfWeek(_messageService, _searchOptions, _userSettingsRepository));
        UserSettings.Add(new UserAddHolidays(_messageService, _searchOptions, _userSettingsRepository));

        UserSettingsWithPlace.Add(new UserPlaces(messageService, _searchOptions, _userSettingsRepository, _placeRepository));

        GetEvents.Add(new ThisWeekEvents(_messageService, _eventService, _botConfiguration.LimitMessagePerSeconds, _placeRepository));
        GetEvents.Add(new NextWeekEvents(_messageService, _eventService, _botConfiguration.LimitMessagePerSeconds, _placeRepository));
        GetEvents.Add(new ThisAndNextWeekEvents(_messageService, _eventService, _botConfiguration.LimitMessagePerSeconds, _placeRepository));
        GetEvents.Add(new ThisMonthEvents(_messageService, _eventService, _botConfiguration.LimitMessagePerSeconds, _placeRepository));
        GetEvents.Add(new Next30DaysEvents(_messageService, _eventService, _botConfiguration.LimitMessagePerSeconds, _placeRepository));
        GetEvents.Add(new Next60DaysEvents(_messageService, _eventService, _botConfiguration.LimitMessagePerSeconds, _placeRepository));
        GetEvents.Add(new ThisYearEvents(_messageService, _eventService, _botConfiguration.LimitMessagePerSeconds, _placeRepository));

        UserSettingsCollbacks.Add(new AddUserPlacesExcludes(_messageService, _searchOptions, _userSettingsRepository));
    }


    public Dictionary<string, IActionMessage> Get()
    {
        var messageActions = new Dictionary<string, IActionMessage>();

        NavigationMessage.ForEach(action => action.InsertTo(messageActions));
        UserSettings.ForEach(userSetting => userSetting.InsertTo(messageActions));
        UserSettingsWithPlace.ForEach(userSetting => userSetting.InsertTo(messageActions));
        GetEvents.ForEach(getEvent => getEvent.InsertTo(messageActions));
        UserSettingsCollbacks.ForEach(collback => collback.InsertTo(messageActions));
        return messageActions;
    }
}
