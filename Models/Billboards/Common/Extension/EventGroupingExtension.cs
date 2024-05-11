using Models.Events;

namespace Models.Billboards.Common.Extension;

public static class EventGroupingExtension
{
    /// <summary>
    /// Grouping by First Link Path
    /// </summary>
    public static IList<Event> DateGrouping(this IList<Event> events)
        => events.GroupBy(@event => @event.Links.FirstOrDefault()?.Path).Select(mainData => {
            var generalData = mainData.First();
            return new Event()
            {
                Billboard = generalData.Billboard,
                Type = generalData.Type,
                Dates = mainData.Where(@event => @event.Dates != null).SelectMany(date => date.Dates)?.Distinct().ToList(),
                EstimatedDates = mainData.Where(@event => @event.EstimatedDates != null).SelectMany(date => date.EstimatedDates)?.Distinct().ToList(),
                Title = generalData.Title,
                NormilizeTitle = generalData.NormilizeTitle,
                NormilizeTitleTerms = generalData.NormilizeTitleTerms,
                ImagePath = generalData.ImagePath,
                Place = generalData.Place,
                Links = generalData.Links,
            };
        }).ToList();
}
