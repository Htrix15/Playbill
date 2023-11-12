﻿using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Service;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;
using System.Globalization;

namespace Models.Billboards.Eventhall;

public class Service : PageParseService
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    public override BillboardTypes BillboardType => BillboardTypes.Eventhall;

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes> searchEventTypes)
    {
        var options = (_options as Options);

        var filteredEventKeys = EventKeys(searchEventTypes);

        var baseSearchUrl = options.BaseSearchUrl;
        var place = options.Place;
        var dateFormat = options.DateFormat;
        var timeFormat = options.TimeFormat;

        var result = new List<Event>();

        try
        {
            var web = new HtmlWeb();

            var doc = await web.LoadFromWebAsync(baseSearchUrl);

            var afishaItems = doc.DocumentNode.SelectNodes(options.ItemsXPath);//options.ItemsContainerXPath);
            foreach (var afishaItem in afishaItems)
            {
                try
                {
                    var dateItem = afishaItem.SelectSingleNode(options.EventDateXPath);
                    var timeItem = afishaItem.SelectSingleNode(options.EventTimeXPath);
                    var time = DateTime.ParseExact(timeItem.InnerText.Trim().Split(" ")[1], timeFormat, CultureInfo.CurrentCulture);
                    var date = DateTime.ParseExact(dateItem.InnerText.Trim().Replace(",",""), dateFormat, CultureInfo.CurrentCulture).AddHours(time.Hour).AddMinutes(time.Minute);
   
                    var eventType = EventTypes.Unidentified;

                    var nameItem = afishaItem.SelectSingleNode(options.EventTitleXPath);
                    var title = nameItem.InnerText.Trim()
                        .Replace("&quot;", "\"")
                        .Replace("&#171;", "«")
                        .Replace("&#187;", "»")
                        .Replace("&amp;", "&");
                    var link = afishaItem.SelectSingleNode("a").Attributes["href"].Value;

                    var imageItem = afishaItem.SelectSingleNode(options.EventImageXPath);
                    var imagePath = imageItem.Attributes["data-original"].Value;

                    result.Add(new Event()
                    {
                        Billboard = BillboardType,
                        Type = eventType,
                        Dates = new List<DateTime>() { date },
                        Title = title,
                        NormilizeTitle = _titleNormalizationService.TitleNormalization(title),
                        NormilizeTitleTerms = _titleNormalizationService.CreateTitleNormalizationTerms(title),
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

        result = result.DateGrouping().ToList();

        return result;
    }

}
