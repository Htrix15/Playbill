using Microsoft.Extensions.Options;
using Playbill.Common.Event;
using Playbill.Services.EventsGrouping;

namespace Playbill.Services.FilterEvents;

public class FilterEventsService
{
    private readonly List<PlaceSynonyms> _synonyms;
    public FilterEventsService(IOptions<PlaceSynonymsOptions> synonyms)
    {
        _synonyms = synonyms.Value?.Synonyms ?? new List<PlaceSynonyms>();
    }

    public IList<Event> FilterEvents(IList<Event> events,
        bool? allPlaces,
        HashSet<string> excludePlacesTerms,
        HashSet<string> inclidePlaces,
        Dictionary<EventTypes, HashSet<string>>? excludeEventsNamesTerms) =>
        events.Where(@event =>
        {
            _synonyms.ForEach(synonyms => synonyms.SetPlaceName(@event));
            var validPlace = excludePlacesTerms.All(excludePlacesTerm => !@event.Place.Contains(excludePlacesTerm, StringComparison.CurrentCultureIgnoreCase)) 
                && ((allPlaces ?? false) || inclidePlaces.Any(inclidePlace => @event.Place.Contains(inclidePlace, StringComparison.CurrentCultureIgnoreCase)));
            var now = DateTime.Now;
            var nowDateOnly = new DateOnly(now.Year, now.Month, now.Day);
            var validTime = (@event.Dates is null && @event.EstimatedDates is not null)
                || (@event.Dates is not null && @event.Dates.Any(date => date > now))
                || (@event.EstimatedDates is not null && @event.EstimatedDates.Any(date => date > nowDateOnly));
            var validName = true;
            if (excludeEventsNamesTerms.TryGetValue(@event.Type, out var excludeNames))
            {
                validName = excludeNames.Any(name => @event.Title?.Contains(name) ?? true);
            }
            return validPlace && validName && validTime;
        }).ToList();
}
