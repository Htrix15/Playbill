using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class EventMessageBase : MessageBase
{
    protected readonly EventService _eventService;
    protected readonly DatePeriods _datePeriod;

    public EventMessageBase(MessageService messageService, 
        EventService eventService, 
        DatePeriods datePeriod) : base(messageService)
    {
        _eventService = eventService;
        _datePeriod = datePeriod;
    }
    public override async Task CreateMessages(BaseParams @params)
    {
        await CreateMessages(@params as GetEventsParams);
    }

    protected async Task CreateMessages(GetEventsParams @params)
    {
        @params.DatePeriod = _datePeriod;
        await _messageService.GetStartSearchMessageAsync(new MessageParams()
        {
            ChatId = @params.ChatId
        });

        var events = await _eventService.GetEvents(@params);
        var eventsMessagesParams = new EventsMessagesParams()
        {
            ChatId = @params.ChatId,
            Events = events
        };
        await _messageService.GetEventMessagesAsync(eventsMessagesParams);
    }
}
