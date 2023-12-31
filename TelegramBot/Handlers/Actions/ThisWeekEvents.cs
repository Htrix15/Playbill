﻿using Models.Places;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class ThisWeekEvents : EventMessageBase
{
    public ThisWeekEvents(MessageService messageService, EventService eventService, int limitMessagePerSeconds, IPlaceRepository placeRepository)
        : base(messageService, eventService, DatePeriods.ThisWeek, limitMessagePerSeconds, placeRepository)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/thisweek";

}
