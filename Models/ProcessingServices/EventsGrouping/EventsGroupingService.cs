using Models.Events;

namespace Models.ProcessingServices.EventsGrouping;

public class EventsGroupingService
{
    private const string _subscription = "абонемент";

    private List<Event> EventDatePreparation(List<Event> events)
    {
        var eventsWithAlternativeDate = new List<Event>();

        foreach (var @event in events)
        {
            if (@event.Dates?.Count > 1)
            {
                var dates = @event.Dates.DistinctBy(date => date.Date).ToList();
                var sessionsGroup = @event.Dates.GroupBy(date => date.Date).ToList();

                dates.Skip(1).ToList().ForEach(date => {
                    var sessions = sessionsGroup.First(group => group.Key == date.Date).Select(date => new TimeOnly(date.Hour, date.Minute, date.Second)).ToList();
                    var hasSessions = sessions.Count > 1;
                    eventsWithAlternativeDate.Add(
                        new Event()
                        {
                            Billboard = @event.Billboard,
                            Type = @event.Type,
                            Dates = new List<DateTime>() { hasSessions ? date.Date : date  },
                            Sessions = hasSessions ? sessions : null,
                            Title = @event.Title,
                            NormilizeTitle = @event.NormilizeTitle,
                            NormilizeTitleTerms = @event.NormilizeTitleTerms,
                            ImagePath = @event.ImagePath,
                            Place = @event.Place,
                            Links = @event.Links
                        });
                    });

                var sessions = sessionsGroup.First(group => group.Key == @event.Date.Value.Date).Select(date => new TimeOnly(date.Hour, date.Minute, date.Second)).ToList();
                var hasSessions = sessions.Count > 1;
                var fiestdate = @event.Dates.First();
                @event.Dates = new List<DateTime>() { hasSessions ? fiestdate.Date : fiestdate };
                @event.Sessions = hasSessions ? sessions : null;
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
                        NormilizeTitle = @event.NormilizeTitle,
                        NormilizeTitleTerms = @event.NormilizeTitleTerms,
                        ImagePath = @event.ImagePath,
                        Place = @event.Place,
                        Links = @event.Links
                    }));
                @event.EstimatedDates = @event.EstimatedDates.Take(1).ToList();
            }

        }

        events.AddRange(eventsWithAlternativeDate);

        var eventsWithoutDate = events.Where(@event => @event.Dates == null || !@event.Dates.Any()).ToList();
        var eventsWithDate = events.Where(@event => @event.Dates != null && @event.Dates.Any()).ToList();

        foreach (var @event in eventsWithoutDate)
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
            }
            else
            {
                @event.Dates = new List<DateTime>()
                {
                    new DateTime(estimatedDate.Year, estimatedDate.Month, estimatedDate.Day)
                };
            }
        }

        return events;
    }

    private bool TitleCompare(Event event1, Event event2)
    {
        if (string.Equals(event1.NormilizeTitle, event2.NormilizeTitle)) return true;

        var event1Subscription = event1.NormilizeTitle.Contains(_subscription);
        var event2Subscription = event2.NormilizeTitle.Contains(_subscription);
        var checkBothSubscription = (event1Subscription && event2Subscription) || (!event1Subscription && !event2Subscription);

        if (event1.NormilizeTitle.Contains(event2.NormilizeTitle) || event2.NormilizeTitle.Contains(event1.NormilizeTitle))
        {
            if (checkBothSubscription)
            {
                return true;
            }
            return false;
        }

        var titleTerms1 = event1.NormilizeTitleTerms;
        var titleTerms1Counts = titleTerms1.Count;

        var titleTerms2 = event2.NormilizeTitleTerms;
        var titleTerms2Counts = titleTerms2.Count;

        var minLegtn = new[] { titleTerms1Counts, titleTerms2Counts }.Min();

        var intersectCount = titleTerms1.Intersect(titleTerms2).Count();
        var tolerance = minLegtn * 0.8;

        if (intersectCount >= tolerance && checkBothSubscription)
        {
            return true;
        }

        return false;
    }

    private List<Event> Grouping(IList<Event> events) 
    {
        var result = new List<Event>();
        var i = 0;

        do
        {
            var newEvent = events[i];
            var foundEvents = events.Skip(i + 1).Where(@event =>
                @event.Date.Value.TimeOfDay == newEvent.Date.Value.TimeOfDay
                && TitleCompare(newEvent, @event)).ToList();
            if (foundEvents.Any())
            {
                newEvent.Links.AddRange(foundEvents.SelectMany(@event => @event.Links));

                var titles = new List<string>() { newEvent.Title };
                titles.AddRange(foundEvents.Select(@event => @event.Title));
                newEvent.Title = titles.OrderByDescending(title => title.Length).First();

                var places = new List<string>() { newEvent.Place };
                places.AddRange(foundEvents.Select(@event => @event.Place));
                newEvent.Place = string.Join(" | ", places.Distinct());
                newEvent.SplitPlace = places.Distinct().Count() > 1;
                foreach (var @event in foundEvents)
                {
                    events.Remove(@event);
                }
            }
            result.Add(newEvent);
            i++;
        }
        while (i < events.Count);

        return result;
    }

    public IList<Event> EventsGrouping(IList<Event> events)
    {
        var _events = EventDatePreparation(events.ToList());

        var result = new List<Event>();

        _events.GroupBy(@event => @event.Date.Value.Date)
            .ToList()
            .ForEach(@event => result.AddRange(Grouping(@event.ToList())));

        return result.OrderBy(@event => @event.Date)
            .ThenBy(@event => @event.Place)
            .ThenBy(@event => @event.Title)
            .ToList();
    }
}
