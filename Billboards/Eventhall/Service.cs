using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Service;
using Playbill.Common;
using Playbill.Common.Event;
using System.Globalization;

namespace Playbill.Billboards.Eventhall;

public class Service : PageParseService
{
    public Service(IOptions<Options> options) : base(options) {}

    public override BillboardTypes BillboardType => BillboardTypes.Eventhall;

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null)
    {
        var options = (_options as Options);

        var filteredEventKeys = EventKeys(searchEventTypes);

        var baseSearchUrl = options.BaseSearchUrl;
        var place = options.Place;
        var dateFormat = options.DateFormat;
        var timeFormat = options.TimeFormat;

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
                    if (afishaItems == null)
                    {
                        continue;
                    }
                    foreach (var afishaItem in afishaItems)
                    {
                        try
                        {
                            var dateTimeItem = afishaItem.SelectSingleNode(options.EventDiteTimeXPath);
                            if (dateTimeItem == null)
                            {
                                continue;
                            }
                            var dates = new List<DateTime>();
                            var dateItem = dateTimeItem.SelectSingleNode(options.EventDateXPath);
                            var dateSourceItems = dateItem.InnerText.Trim().Replace(",", "").Split(' ');
                            var dateSourceLength = dateSourceItems.Length;
                            var datesSources = dateSourceLength <= 4
                                //for parse 02 октября 2023, понедельник 
                                ? new List<string>() { $"{dateSourceItems[0]} {dateSourceItems[1]} {dateSourceItems[2]}" }
                                //for parse 18, 19, 20 августа 2023
                                : dateSourceItems.Take(dateSourceLength - 2).Select(date => $"{date} {dateSourceItems[dateSourceLength - 2]} {dateSourceItems[dateSourceLength - 1]}");
                           
                            var timeItem = dateTimeItem.SelectSingleNode(options.EventTimeXPath);
                            var time = DateTime.ParseExact(timeItem.InnerText.Trim(), timeFormat, CultureInfo.CurrentCulture);

                            foreach (var dateSource in datesSources)
                            {
                                var date = DateTime.ParseExact(dateSource, dateFormat, CultureInfo.CurrentCulture).AddHours(time.Hour).AddMinutes(time.Minute);
                                var chackDate = new DateOnly(date.Year, date.Month, date.Day);

                                if (eventDateIntervals.Any(eventDateInterval => chackDate >= eventDateInterval.StartDate 
                                    && chackDate <= eventDateInterval.EndDate
                                    && date > DateTime.Now))
                                {
                                    dates.Add(date);
                                }
                            }

                            if (!dates.Any()) { continue; }

                            var eventType = eventKeys.Key;

                            var nameItem = afishaItem.SelectSingleNode(options.EventTitleXPath);
                            var title = nameItem.InnerText.Trim()
                                .Replace("&quot;", "\"")
                                .Replace("&#171;", "«")
                                .Replace("&#187;", "»");
                            var link = nameItem.Attributes["href"].Value;

                            var imageItem = afishaItem.SelectSingleNode(options.EventImageXPath);
                            var imagePath = imageItem.Attributes["src"].Value;

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
