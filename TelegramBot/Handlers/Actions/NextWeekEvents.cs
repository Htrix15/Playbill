using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class NextWeekEvents : EventMessageBase
{
    public NextWeekEvents(MessageService messageService, EventService eventService) : base(messageService, eventService, DatePeriods.NextWeek)
    {
    }
    public override string Command => Commands.NextWeek;
}
