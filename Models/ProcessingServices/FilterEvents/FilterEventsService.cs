﻿using Models.Events;

namespace Models.ProcessingServices.FilterEvents;

public class FilterEventsService
{
    public List<Event> FilterEvents(List<Event> events,
        bool? allPlaces,
        HashSet<string> excludePlacesTerms,
        HashSet<string> inclidePlaces,
        Dictionary<EventTypes, HashSet<string>> excludeEventsNamesTerms) =>
        events.Where(@event =>
        {
            var validPlace = !excludePlacesTerms.Any(excludePlacesTerm => @event.Place.Equals(excludePlacesTerm, StringComparison.CurrentCultureIgnoreCase)) 
                && ((allPlaces ?? false) || inclidePlaces.Any(inclidePlace => @event.Place.Equals(inclidePlace, StringComparison.CurrentCultureIgnoreCase)));
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
