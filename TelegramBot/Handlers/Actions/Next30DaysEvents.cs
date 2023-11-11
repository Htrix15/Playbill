using Models.Places;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Next30DaysEvents : EventMessageBase
{
    public Next30DaysEvents(MessageService messageService, EventService eventService, int limitMessagePerSeconds, IPlaceRepository placeRepository) 
        : base(messageService, eventService, DatePeriods.Next30Days, limitMessagePerSeconds, placeRepository)
    {
    }

    public override string Command => GetCommand();
    public static new string GetCommand() => "/next30days";

}
