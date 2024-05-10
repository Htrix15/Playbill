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

namespace Models.Billboards.Eventhall;

public class Service : PageParseService
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    public override BillboardTypes BillboardType => BillboardTypes.Eventhall;

    protected override List<DateTime>? GetEventDates(HtmlNode afishaItem,
        PageParseOptions options,
        string title = "")
    {
        try
        {
            var dates = new List<DateTime>();

            var dateFormat = (options as Options).DateFormat;
            var timeFormat = (options as Options).TimeFormat;

            var dateItems = afishaItem.SelectNodes((options as Options).EventDiteTimeXPath);

            if (dateItems is null)
            {
                return dates;
            }

            for (int i = 0; i < dateItems.Count(); i += 3)
            {
                var time = DateTime.ParseExact(dateItems[i + 1].InnerText.Trim().Split(" ")[1], timeFormat, CultureInfo.CurrentCulture);
                var date = DateTime.ParseExact(dateItems[i].InnerText.Trim().Replace(",", ""), dateFormat, CultureInfo.CurrentCulture).AddHours(time.Hour).AddMinutes(time.Minute);
                dates.Add(date);
            }
            return dates;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {title} - {PageBlock.Date}): {exception.Message}");
            return null;
        }
    }
    protected string? GetImagePath(HtmlNode afishaItem, string eventImageXPath, string title)
    {
        try
        {
            var imageItems = afishaItem.SelectNodes(eventImageXPath);

            string? imagePath = null;
            foreach (var imageItem in imageItems)
            {
                var imageAttribute = imageItem.Attributes["data-original"];
                if (imageAttribute == null) continue;
                imagePath = imageAttribute.Value;
            }

            return imagePath;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {title} - {PageBlock.Image}): {exception.Message}");
            return null;
        }
    }

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes> searchEventTypes)
    {
        var options = (_options as Options);
        var result = new List<Event>();

        try
        {
            var doc = await GetBuilbordPage(options.BaseSearchUrl);
            var afishaItems = GetAfishaItems(doc.DocumentNode, options.ItemsXPath);
           
            foreach (var afishaItem in afishaItems)
            {
                var title = GetTitle(afishaItem, options.EventTitleXPath);
                var imagePath = GetImagePath(afishaItem, options.EventImageXPath, title: title);

                if (title is null && imagePath is null) continue;

                var dates = GetEventDates(afishaItem, options, title: title);
                var substandard = dates is null;
                dates = FilterDate(dates, eventDateIntervals);
                if (!substandard && dates.Count == 0) continue;

                var link = GetLink(afishaItem, options.LinkXPath, title: title);
                  
                result.Add(new Event()
                {
                    Billboard = BillboardType,
                    Type = EventTypes.Unidentified,
                    Dates = dates,
                    Title = title,
                    NormilizeTitle = _titleNormalizationService.TitleNormalization(title),
                    NormilizeTitleTerms = _titleNormalizationService.CreateTitleNormalizationTerms(title),
                    ImagePath = imagePath,
                    Place = options.Place,
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
            Console.WriteLine($"Fail parse page ({BillboardType}): {exception.Message}");
        }

        result = result.DateGrouping().ToList();

        return result;
    }

}
