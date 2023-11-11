﻿using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class Next60DaysEvents : EventMessageBase
{
    public Next60DaysEvents(MessageService messageService, EventService eventService) : base(messageService, eventService, DatePeriods.Next60Days)
    {
    }

    public override string Command => GetCommand();
    public static new string GetCommand() => "/next60days";

}