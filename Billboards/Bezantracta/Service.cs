using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Service;
using Playbill.Common;
using Playbill.Common.Event;
using System.Globalization;

namespace Playbill.Billboards.Bezantracta;

public class Service : PageParseService
{
    public Service(IOptions<Options> options) : base(options) {}

    public override BillboardTypes BillboardType => BillboardTypes.Bezantracta;

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null)
    {
        var options = (_options as Options);

        var filteredEventKeys = EventKeys(searchEventTypes);

        var baseSearchUrl = options.BaseSearchUrl;
        var baseLinkUrl = options.BaseLinkUrl;

        var result = new List<Event>();

        foreach (var eventKeys in filteredEventKeys)
        {
            foreach (var eventKey in eventKeys.Value)
            {
                try
                {
                    var web = new HtmlWeb();
                    var doc = await web.LoadFromWebAsync(baseSearchUrl + eventKey);

                    var fullAfisha = doc.DocumentNode.SelectSingleNode(options.ItemsContainerXPath);
                    var afishaItems = fullAfisha.SelectNodes(options.ItemsXPath);
                    foreach (var afishaItem in afishaItems)
                    {
                        try
                        {
                            var dateItem = afishaItem.SelectSingleNode(options.EventDateXPath);
                            var date = DateTime.ParseExact(dateItem.InnerText.Trim(), options.DateFormat, CultureInfo.CurrentCulture);
                            var chackDate = new DateOnly(date.Year, date.Month, date.Day);

                            if (!eventDateIntervals.Any(eventDateInterval => chackDate >= eventDateInterval.StartDate && chackDate <= eventDateInterval.EndDate) || date < DateTime.Now)
                            {
                                continue;
                            }

                            var eventType = eventKeys.Key;

                            var nameItem = afishaItem.SelectSingleNode(options.EventTitleXPath);
                            var title = nameItem.InnerText.Trim().Replace("&quot;", "\"");

                            var placeItem = afishaItem.SelectSingleNode(options.PlaceXPath);
                            var place = placeItem.InnerText.Trim();

                            var imageItem = afishaItem.SelectSingleNode(options.EventImageXPath);
                            var imagePath = baseLinkUrl + imageItem.Attributes["src"].Value;

                            var linkItem = afishaItem.SelectSingleNode(options.LinkXPath);
                            var link = baseLinkUrl + linkItem.Attributes["href"].Value;
                            result.Add(new Event()
                            {
                                Billboard = BillboardType,
                                Type = eventType,
                                Dates = new List<DateTime>() { date },
                                Title = title,
                                ImagePath = imagePath,
                                Place = place,
                                Links = new List<EventLink>()
                                {
                                    new EventLink()
                                    {
                                        BillboardType = BillboardType,
                                        Path = link
                                    }
                                }
                            });
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"Fail parse items: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Fail parse page: {exception.Message}");
                }
            }
        }


        result = result.DateGrouping().ToList();
        return result;
    }
}
