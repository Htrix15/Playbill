﻿using Models.Events;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class EventMessageBase : MessageBase
{
    protected readonly EventService _eventService;
    protected readonly DatePeriods _datePeriod;

    public EventMessageBase(MessageService messageService, 
        EventService eventService, 
        DatePeriods datePeriod) : base(messageService)
    {
        _eventService = eventService;
        _datePeriod = datePeriod;
    }
    public override async Task CreateMessages(Update update)
    {
        var @params = update.GetGetEventsParams(_datePeriod);

        await _messageService.SendMessageAsync(@params.ChatId, "Начат поиск событий...");

        var events = await _eventService.GetEvents(@params);
        foreach (var @event in events)
        {
            var caption = GetCaptionFromEvent(@event);
            var buttons = MarkupHelper.GetEventButtons(@event.Links);
            await _messageService.SendMessageWithPhotoAsync(@params.ChatId, caption, @event.ImagePath, buttons, false);

        }
        await _messageService.SendMessageAsync(@params.ChatId, "Поиск завершен!", MarkupHelper.GetStartButtons());
    }

    private string GetCaptionFromEvent(Event @event)
    {
        var dateMask = @event.Date!.Value.Hour != 0 ? "dd MMMM HH:mm (dddd)" : "dd MMMM (dddd)";
        var times = @event.Sessions?.Any() ?? false
            ? $"({string.Join(", ", @event.Sessions.Select(session => session.ToString("HH:mm")))})"
            : "";
        return $"*{MessageService.TextNormalization(@event.Title)}*" +
            $"\n{@event.Type}" +
            $"\n*{MessageService.TextNormalization(@event.Date.Value.ToString(dateMask))}*" +
            $"\n{MessageService.TextNormalization(times)}" +
            $"\n{MessageService.TextNormalization(@event.Place)}";
    }
}
