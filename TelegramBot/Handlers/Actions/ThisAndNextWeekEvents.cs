using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class ThisAndNextWeekEvents : EventMessageBase
{
    public ThisAndNextWeekEvents(MessageService messageService, EventService eventService)
        : base(messageService, eventService, DatePeriods.ThisAndNextWeek)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/thisandnextweek";
}
