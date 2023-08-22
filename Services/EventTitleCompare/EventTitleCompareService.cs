using Microsoft.Extensions.Options;
using Playbill.Common;
using Playbill.Common.Event;
using System.Text.RegularExpressions;

namespace Playbill.Services.EventTitleCompare;

public class EventTitleCompareService
{
    private char[] _separators { get; set; }
    private List<string> _wordEndings { get; set; }

    public EventTitleCompareService(IOptions<TitleCompareOptions> titleCompareOptions)
    {
        _separators = titleCompareOptions.Value.Separators.ToArray();
        _wordEndings = titleCompareOptions.Value.WordEndings.OrderByDescending(wordEnding => wordEnding.Length).ToList();
    }

    private Func<string, List<string>, string> TermNormalization = (string term, List<string> wordEndings) =>
    {
        term = Regex.Replace(term, RegexPatterns.NormilizeTitle, "");
        if (term.Length <= 3)
        {
            return term;
        }
        foreach (var wordEnding in wordEndings)
        {
            if (term.EndsWith(wordEnding))
            {
                return term.Substring(0, term.Length - wordEnding.Length);
            }
        }
        return term;
    };

    private bool TitleCompare(Event event1, Event event2)
    {
        if (String.Equals(event1.NormilizeTitle, event2.NormilizeTitle)) return true;
        if (event1.NormilizeTitle.Contains(event2.NormilizeTitle) || event2.NormilizeTitle.Contains(event1.NormilizeTitle)) return true;
        
        var titleTerms1 = event1.Title.Split(_separators)
            .Where(term => !string.IsNullOrEmpty(term))
            .Select(term => TermNormalization(term, _wordEndings))
            .ToList();
        var titleTerms1Counts = titleTerms1.Count;

        var titleTerms2 = event2.Title.Split(_separators)
            .Where(term => !string.IsNullOrEmpty(term))
            .Select(term => TermNormalization(term, _wordEndings))
            .ToList();
        var titleTerms2Counts = titleTerms2.Count;

        var minLegtn = new[] { titleTerms1Counts, titleTerms2Counts }.Min();

        var intersectCount = titleTerms1.Intersect(titleTerms2).Count();
        var tolerance = minLegtn * 0.8;

        if (intersectCount >= tolerance)
        {
            return true;
        }

        return false;
    }

    public List<Event> GropingByTitle(List<Event> events)
    {
        var result = new List<Event>();
        var search = false;
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

                foreach (var @event in foundEvents) {
                    events.Remove(@event);
                }
            }
            result.Add(newEvent);
            i++;
            continue;
        }
        while (i < events.Count);

        return result;
    }
}
