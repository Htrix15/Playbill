using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Service;
using Playbill.Common;
using Playbill.Common.Event;
using System;
using System.Globalization;

namespace Playbill.Billboards.Quickticket;

public class Service : PageParseService
{
    public Service(IOptions<Options> options) : base(options){}

    public override BillboardTypes BillboardType => BillboardTypes.Quickticket;

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
                    var placeItem = doc.DocumentNode.SelectSingleNode(options.PlaceXPath);
                    var place = placeItem.InnerText.Trim();
                    var fullAfisha = doc.DocumentNode.SelectSingleNode(options.ItemsContainerXPath);
                    var afishaItems = fullAfisha.SelectNodes(options.ItemsXPath);
                    foreach (var afishaItem in afishaItems)
                    {
                        try
                        {
                            var dates = new List<DateTime>();

                            var dateContainers = afishaItem.SelectNodes(options.DatesXPath);
                            foreach (var dateContainer in dateContainers)
                            {
                                var dateItems = dateContainer.SelectNodes(options.EventDateXPath);
                                if (dateItems == null) { continue; }
                                foreach (var dateItem in dateItems)
                                {
                                    if (dateItem == null) { continue; }
                                    var date = DateTime.ParseExact(dateItem.InnerText.Trim(), options.DateFormat, CultureInfo.CurrentCulture);
                                    var chackDate = new DateOnly(date.Year, date.Month, date.Day);
                                    if (eventDateIntervals.Any(eventDateInterval => chackDate >= eventDateInterval.StartDate && chackDate <= eventDateInterval.EndDate) && date > DateTime.Now)
                                    {
                                        dates.Add(date);
                                    }
                                }
                            }
                            if (!dates.Any()) { continue; }

                            var nameItem = afishaItem.SelectSingleNode(options.EventTitleXPath);
                            var title = nameItem.InnerText.Trim().Replace("&quot;", "\"");

                            var eventType = eventKeys.Key;

                            var imageItem = afishaItem.SelectSingleNode(options.EventImageXPath);
                            var imagePath = imageItem.Attributes["src"].Value;

                            var linkItem = afishaItem.SelectSingleNode(options.LinkXPath);
                            var link = baseLinkUrl + linkItem.Attributes["href"].Value;
                            result.Add(new Event()
                            {
                                Billboard = BillboardType,
                                Type = eventType,
                                Dates = dates,
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
