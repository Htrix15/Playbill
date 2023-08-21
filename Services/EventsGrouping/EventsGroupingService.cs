using Microsoft.Extensions.Options;
using Playbill.Common.Event;

namespace Playbill.Services.EventsGrouping;

public class EventsGroupingService
{
    private List<PlaceSynonyms> _synonyms { get; init; }
    public EventsGroupingService(IOptions<PlaceSynonymsOptions> synonyms )
    {
        _synonyms = synonyms.Value?.Synonyms ?? new List<PlaceSynonyms>();
    }
    public IList<Event> EventsGrouping(IList<Event> events)
    {
        var _events = events.ToList();
        var eventsWithAlternativeDate = new List<Event>();
        foreach(var @event in _events)
        {
            _synonyms.ForEach(synonyms => synonyms.SetPlaceName(@event));

            if (@event.Dates?.Count > 1)
            {
                @event.Dates.Skip(1).ToList().ForEach(date => eventsWithAlternativeDate.Add(
                    new Event()
                    {
                        Billboard = @event.Billboard,
                        Type = @event.Type,
                        Dates = new List<DateTime>() { date },
                        Title = @event.Title,
                        ImagePath = @event.ImagePath,
                        Place = @event.Place,
                        Links = @event.Links
                    }));
                @event.Dates = @event.Dates.Take(1).ToList();
            }
            if (@event.EstimatedDates?.Count > 1)
            {
                @event.EstimatedDates.Skip(1).ToList().ForEach(date => eventsWithAlternativeDate.Add(
                    new Event()
                    {
                        Billboard = @event.Billboard,
                        Type = @event.Type,
                        Dates = null,
                        EstimatedDates = new List<DateOnly>() { date },
                        Title = @event.Title,
                        ImagePath = @event.ImagePath,
                        Place = @event.Place,
                        Links = @event.Links
                    }));
                @event.EstimatedDates = @event.EstimatedDates.Take(1).ToList();
            }
        }
        _events.AddRange(eventsWithAlternativeDate);

        var eventsWithoutDate = _events.Where(@event => @event.Dates == null || !@event.Dates.Any()).ToList();
        var eventsWithDate = _events.Where(@event => @event.Dates != null && @event.Dates.Any()).ToList();

        foreach(var @event in eventsWithoutDate)
        {
            var estimatedDate = @event.EstimatedDates.First();
            var foundDate = eventsWithDate.FirstOrDefault(eventWithDate =>
            {
                var date = eventWithDate.Date.Value;
                return string.Equals(eventWithDate.Place, @event.Place) && estimatedDate == new DateOnly(date.Year, date.Month, date.Day);
            })?.Dates?.First();
            if (foundDate != null)
            {
                @event.Dates = new List<DateTime>()
                {
                    foundDate.Value
                };
            } else
            {
                @event.Dates = new List<DateTime>()
                {
                    new DateTime(estimatedDate.Year, estimatedDate.Month, estimatedDate.Day)
                };
            }
        }
        var groupedEvent = _events
            .GroupBy(@event => new { @event.Place, @event.Date, @event.Title })
            .Select(@event => new Event()
            {
                Billboard = @event.First().Billboard,
                Type = @event.FirstOrDefault(e => e.Type != EventTypes.Unidentified)?.Type ?? EventTypes.Unidentified,
                Dates = new List<DateTime>() { @event.Key.Date!.Value },
                Title = @event.Key.Title,
                ImagePath = @event.FirstOrDefault(e => e.ImagePath != null)?.ImagePath,
                Place = @event.Key.Place,
                Links = @event.SelectMany(e => e.Links).ToList()
            }
            )
            .OrderBy(@event => @event.Date)
            .ToList();

        return groupedEvent;
    }
}
