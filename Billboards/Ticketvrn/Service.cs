﻿using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Service;
using Playbill.Common;
using Playbill.Common.Event;
using Playbill.Services.TitleNormalization.Common;
using System.Globalization;

namespace Playbill.Billboards.Ticketvrn;

public class Service : PageParseService
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    public override BillboardTypes BillboardType => BillboardTypes.Ticketvrn;

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
    {
        var options = (_options as Options);
        var baseSearchUrl = options.BaseSearchUrl;
        var baseLinkUrl = options.BaseLinkUrl;

        var result = new List<Event>();

        try
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(baseSearchUrl);

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

                    var eventType = EventTypes.Unidentified;

                    var nameItem = afishaItem.SelectSingleNode(options.EventTitleXPath);
                    var title = nameItem.InnerText.Trim().Replace("&quot;", "\"");

                    var placeItem = afishaItem.SelectSingleNode(options.PlaceXPath);
                    var place = placeItem.InnerText.Trim();

                    var imageItem = afishaItem.SelectSingleNode(options.EventImageXPath);
                    var imagePath = baseLinkUrl + imageItem.Attributes["src"].Value;

                    var link = baseLinkUrl;
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

        return result;
    }
}
