using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class ThisMonthEvents : EventMessageBase
{
    public ThisMonthEvents(MessageService messageService, EventService eventService) : base(messageService, eventService, DatePeriods.ThisMonth)
    {
    }

    public override string Command => Commands.ThisMonth;
}
