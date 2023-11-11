using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class ThisYearEvents : EventMessageBase
{
    public ThisYearEvents(MessageService messageService, EventService eventService, int limitMessagePerSeconds) 
        : base(messageService, eventService, DatePeriods.ThisYear, limitMessagePerSeconds)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/thisyear";
}
