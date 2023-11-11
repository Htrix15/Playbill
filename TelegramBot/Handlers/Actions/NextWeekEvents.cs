using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class NextWeekEvents : EventMessageBase
{
    public NextWeekEvents(MessageService messageService, EventService eventService, int limitMessagePerSeconds) 
        : base(messageService, eventService, DatePeriods.NextWeek, limitMessagePerSeconds)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/nextweek";
}
