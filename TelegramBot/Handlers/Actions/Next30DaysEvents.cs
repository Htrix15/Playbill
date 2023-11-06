using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Next30DaysEvents : EventMessageBase
{
    public Next30DaysEvents(MessageService messageService, EventService eventService) : base(messageService, eventService, DatePeriods.Next30Days)
    {
    }

    public override string Command => Commands.Next30Days;
}
