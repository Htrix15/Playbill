﻿using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Service;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;
using System.Globalization;

namespace Models.Billboards.Ticketvrn;

public class Service : PageParseService
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    public override BillboardTypes BillboardType => BillboardTypes.Ticketvrn;

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
    {
        var result = new List<Event>();

        if (!searchEventTypes.Contains(EventTypes.Unidentified))
        {
            return result;
        }

        var options = (_options as Options);
        var baseSearchUrl = options.BaseSearchUrl;
        var baseLinkUrl = options.BaseLinkUrl;

        try
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(baseSearchUrl);

            var fullAfisha = doc.DocumentNode.SelectSingleNode(options.ItemsContainerXPath);
            if (fullAfisha is null)
            {
                return result;
            }
            var afishaItems = fullAfisha.SelectNodes(options.ItemsXPath);
            if (afishaItems is null)
            {
                return result;
            }
            foreach (var afishaItem in afishaItems)
            {
                try
                {
                    var dateItem = afishaItem.SelectSingleNode(options.EventDateXPath);
                    if (dateItem is null)
                    {
                        continue;
                    }
                    var dateItemText = dateItem.InnerText.Trim();
                    var date = DateTime.Now;
                    try
                    {
                        date = DateTime.ParseExact(dateItemText, options.DateFormat, CultureInfo.CurrentCulture);
                        if (DateTime.Now.Month > 10 && date.Month < 3)
                        {
                            date = date.AddYears(1);
                        }
                    }
                    catch (FormatException)
                    {
                        dateItemText += $",{date.AddYears(1).Year}";
                        date = DateTime.ParseExact(dateItemText, options.DateFormat + ",yyyy", CultureInfo.CurrentCulture);
                    }

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
                    Console.WriteLine($"Fail parse items (Ticketvrn): {exception.Message}");
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse page (Ticketvrn): {exception.Message}");
        }

        return result;
    }
}
