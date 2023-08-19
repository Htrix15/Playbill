
namespace Playbill.Billboards.Common.Extension;

public static class EventGroupingExtension
{
    /// <summary>
    /// Grouping by First Link Path
    /// </summary>
    public static IList<Event.Event> DateGrouping(this IList<Event.Event> events)
        => events.GroupBy(@event => @event.Links.FirstOrDefault()?.Path).Select(mainDate => {
            var generalData = mainDate.First();
            return new Event.Event()
            {
                Billboard = generalData.Billboard,
                Type = generalData.Type,
                Dates = mainDate.Where(@event => @event.Dates != null).SelectMany(date => date.Dates)?.ToList(),
                EstimatedDates = mainDate.Where(@event => @event.EstimatedDates != null).SelectMany(date => date.EstimatedDates)?.ToList(),
                Title = generalData.Title,
                ImagePath = generalData.ImagePath,
                Place = generalData.Place,
                Links = generalData.Links,
            };
        }).ToList();
}
