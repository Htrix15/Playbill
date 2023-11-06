using TelegramBot.Handlers.Actions;
using TelegramBot.Handlers.Actions.Common;

namespace TelegramBot.Services;

public class MessageActionsService
{
    private readonly MessageService _messageService;
    private readonly UserSettingsService _userSettingsService;
    private readonly EventService _eventService;

    private readonly List<MessageBase> Actions = new();
    private readonly List<SettingsMessageBase> UserSettings = new();
    private readonly List<EventMessageBase> GetEvents = new();

    public MessageActionsService(MessageService messageService, UserSettingsService userSettingsService, EventService eventService)
    {
        _messageService = messageService;
        _userSettingsService = userSettingsService;
        _eventService = eventService;

        Actions.Add(new Start(_messageService));
        Actions.Add(new Search(_messageService));
        Actions.Add(new Settings(_messageService));

        UserSettings.Add(new UserBillboards(_messageService, _userSettingsService));
        UserSettings.Add(new UserEventTypes(_messageService, _userSettingsService));
        UserSettings.Add(new UserDaysOfWeek(_messageService, _userSettingsService));
        UserSettings.Add(new UserAddHolidays(_messageService, _userSettingsService));

        GetEvents.Add(new ThisWeekEvents(_messageService, _eventService));
        GetEvents.Add(new ThisMonthEvents(_messageService, _eventService));
        GetEvents.Add(new Next30DaysEvents(_messageService, _eventService));
        GetEvents.Add(new ThisYearEvents(_messageService, _eventService));
    }


    public Dictionary<string, IActionMessage> Get()
    {
        var messageActions = new Dictionary<string, IActionMessage>();

        Actions.ForEach(action => action.InsertTo(messageActions));
        UserSettings.ForEach(userSetting => userSetting.InsertTo(messageActions));
        GetEvents.ForEach(getEvent => getEvent.InsertTo(messageActions));
        return messageActions;
    }

    public List<string> GetSettingsKeys() => UserSettings.Select(userSetting => userSetting.Command).ToList();
    public List<string> GetEventsKeys() => GetEvents.Select(userSetting => userSetting.Command).ToList();
}
