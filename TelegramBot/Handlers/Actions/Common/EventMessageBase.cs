using Models.Events;
using Models.Places;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using System.Diagnostics;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Helpers;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class EventMessageBase : MessageBase
{
    protected readonly EventService _eventService;
    private readonly IPlaceRepository _placeRepository;
    protected readonly DatePeriods _datePeriod;
    private readonly int _limitMessagePerSeconds;
    public EventMessageBase(MessageService messageService, 
        EventService eventService, 
        DatePeriods datePeriod,
        int limitMessagePerSeconds,
        IPlaceRepository placeRepository) : base(messageService)
    {
        _eventService = eventService;
        _datePeriod = datePeriod;
        _limitMessagePerSeconds = limitMessagePerSeconds;
        _placeRepository = placeRepository;
    }
    public override async Task CreateMessages(Update update)
    {
        var @params = update.GetGetEventsParams(_datePeriod);

        await _messageService.SendMessageAsync(@params.ChatId, "Начат поиск событий...");

        var events = await _eventService.GetEvents(@params);
        events = events.Where(e => !e.Substandard).ToList();
        var sentEventCount = 0;
        var sendingTimeControl = new Stopwatch();
        sendingTimeControl.Start();
        foreach (var @event in events)
        {
            var caption = GetCaptionFromEvent(@event);
            var buttons = MarkupHelper.GetEventButtons(@event.Links);
            try
            {
                await _messageService.SendMessageWithPhotoAsync(@params.ChatId, caption, @event.ImagePath, buttons, false);
            }
            catch (ApiRequestException exception) when (exception.ErrorCode == 429)
            {
                await Task.Delay(1000 * exception.Parameters?.RetryAfter ?? 1);
                await _messageService.SendMessageAsync(@params.ChatId, "Телеге устала и попросила немного отдохнуть :(");
                sentEventCount++;
                await _messageService.SendMessageWithPhotoAsync(@params.ChatId, caption, @event.ImagePath, buttons, false);
            }

            sentEventCount++;
            if (sentEventCount == _limitMessagePerSeconds)
            {
                sendingTimeControl.Stop();
                int elapsedMilliseconds = (int)sendingTimeControl.ElapsedMilliseconds;
                if (elapsedMilliseconds < 1000)
                {
                    await Task.Delay(1000 - elapsedMilliseconds + 100);
                }
                sentEventCount = 0;
                sendingTimeControl.Restart();
            }
        }
        sendingTimeControl.Stop();

        var places = await _placeRepository.GetPlacesAsync(events.Select(@event => @event.Place).Distinct());

        var toogleButtons = MarkupHelper.CreateToogleButtons(places.Select(place => new UserSettingsParams.Setting() { 
            Id = place.Id, 
            Label = place.Name 
        }).ToList(), CommandKeyHelper.Create(AddUserPlacesExcludes.GetCommand()));

        await _messageService.SendMessageAsync(@params.ChatId, "Поиск завершен!" +
            "\nОтметьте те места, откуда вы больше не хотите видить события." +
            "\nВ настройках можно будет вернуть обратно", toogleButtons.ToInlineKeyboardMarkup());

        await _messageService.SendMessageAsync(@params.ChatId, "Что дальше?", MarkupHelper.GetStartButtons());
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
