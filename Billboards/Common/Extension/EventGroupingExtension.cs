using Playbill.Common.Event;

namespace Playbill.Billboards.Common.Extension;

public static class EventGroupingExtension
{
    /// <summary>
    /// Grouping by First Link Path
    /// </summary>
    public static IList<Event> DateGrouping(this IList<Event> events)
        => events.GroupBy(@event => @event.Links.FirstOrDefault()?.Path).Select(mainDate => {
            var generalData = mainDate.First();
            return new Event()
            {
                Billboard = generalData.Billboard,
                Type = generalData.Type,
                Dates = mainDate.Where(@event => @event.Dates != null).SelectMany(date => date.Dates)?.Distinct().ToList(),
                EstimatedDates = mainDate.Where(@event => @event.EstimatedDates != null).SelectMany(date => date.EstimatedDates)?.Distinct().ToList(),
                Title = generalData.Title,
                NormilizeTitle = generalData.NormilizeTitle,
                NormilizeTitleTerms = generalData.NormilizeTitleTerms,
                ImagePath = generalData.ImagePath,
                Place = generalData.Place,
                Links = generalData.Links,
            };
        }).ToList();
}
