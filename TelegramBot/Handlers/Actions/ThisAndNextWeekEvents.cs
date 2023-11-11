using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class ThisAndNextWeekEvents : EventMessageBase
{
    public ThisAndNextWeekEvents(MessageService messageService, EventService eventService, int limitMessagePerSeconds)
        : base(messageService, eventService, DatePeriods.ThisAndNextWeek, limitMessagePerSeconds)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/thisandnextweek";
}
