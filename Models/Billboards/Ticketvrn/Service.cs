﻿using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Exceptions;
using Models.Billboards.Common.Options;
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

    protected override List<DateTime>? GetEventDates(HtmlNode afishaItem,
       PageParseOptions options,
       string title = "")
    {
        try
        {
            var dateItem = afishaItem.SelectSingleNode(options.EventDateXPath);
            if (dateItem is null)
            {
                return null;
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

            return new List<DateTime>() { date };
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {title} - {PageBlock.Image}): {exception.Message}");
            return null;
        }
    }

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
    {
        var result = new List<Event>();

        if (!searchEventTypes.Contains(EventTypes.Unidentified))
        {
            return result;
        }

        var options = (_options as Options);

        try
        {
            var doc = await GetBuilbordPage(options.BaseSearchUrl);
            var fullAfisha = GetFullAfisha(doc, options.ItemsContainerXPath);
            var afishaItems = GetAfishaItems(fullAfisha, options.ItemsXPath);

            foreach (var afishaItem in afishaItems)
            {
                string? title = null;

                try
                {
                    title = GetTitle(afishaItem, options.EventTitleXPath);
                    var imagePath = GetImagePath(afishaItem, options.EventImageXPath, options.BaseLinkUrl, title: title);

                    if (title is null && imagePath is null) continue;

                    var dates = GetEventDates(afishaItem, options, title: title);
                    var substandard = dates is null;
                    dates = FilterDate(dates, eventDateIntervals);
                    if (!substandard && dates.Count == 0) continue;

                    var place = GetPlace(afishaItem, options.PlaceXPath, title: title);
  
                    result.Add(new Event()
                    {
                        Billboard = BillboardType,
                        Type = EventTypes.Unidentified,
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
                                Path = options.BaseLinkUrl
                            }
                        },
                        Substandard = substandard
                    });
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Fail parse items ({BillboardType} - {title}): {exception.Message}");
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse page ({BillboardType}): {exception.Message}");
        }

        return result;
    }
}
