using Models.Places;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class ThisMonthEvents : EventMessageBase
{
    public ThisMonthEvents(MessageService messageService, EventService eventService, int limitMessagePerSeconds, IPlaceRepository placeRepository) 
        : base(messageService, eventService, DatePeriods.ThisMonth, limitMessagePerSeconds, placeRepository)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/thismonth";
}
