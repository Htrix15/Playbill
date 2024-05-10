﻿using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Exceptions;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Options;
using Models.Billboards.Common.Service;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;
using System.Globalization;

namespace Models.Billboards.Bezantracta;

public class Service : PageParseService
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    public override BillboardTypes BillboardType => BillboardTypes.Bezantracta;

    protected override List<DateTime>? GetEventDates(HtmlNode afishaItem,
      PageParseOptions options,
      string title)
    {
        try
        {
            var dates = new List<DateTime>();
            var dateItem = afishaItem.SelectSingleNode(options.EventDateXPath);
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

            dates.Add(date);
            return new List<DateTime>() { date };
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {title} - {PageBlock.Date}): {exception.Message}");
            return null;
        }
    }

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
    {
        var options = (_options as Options);

        var filteredEventKeys = EventKeys(searchEventTypes);
        var result = new List<Event>();

        foreach (var eventKeys in filteredEventKeys)
        {
            foreach (var eventKey in eventKeys.Value)
            {
                try
                {
                    var doc = await GetBuilbordPage(options.BaseSearchUrl + eventKey);
                    var fullAfisha = GetFullAfisha(doc, options.ItemsContainerXPath);
                    var afishaItems = GetAfishaItems(fullAfisha, options.ItemsXPath);

                    foreach (var afishaItem in afishaItems)
                    {
                        var title = GetTitle(afishaItem, options.EventTitleXPath);
                        var imagePath = GetImagePath(afishaItem, options.EventImageXPath, options.BaseLinkUrl, title);

                        if (title is null && imagePath is null) continue;

                        var dates = GetEventDates(afishaItem, options, title);
                        var substandard = dates is null;
                        dates = FilterDate(dates, eventDateIntervals);
                        if (!substandard && dates.Count == 0) continue;

                        var place = GetPlace(afishaItem, options.PlaceXPath, title: title);          
                        var link = GetLink(afishaItem, options.LinkXPath, options.BaseLinkUrl, title: title);

                        result.Add(new Event()
                        {
                            Billboard = BillboardType,
                            Type = eventKeys.Key,
                            Dates = dates,
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
                            },
                            Substandard = substandard
                        });
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Fail parse page ({BillboardType} - {eventKey}): {exception.Message}");
                }
            }
        }

        result = result.DateGrouping().ToList();
        return result;
    }
}
