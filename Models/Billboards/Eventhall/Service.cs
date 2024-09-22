using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Exceptions;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Logging;
using Models.Billboards.Common.Options;
using Models.Billboards.Common.Service;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;
using System.Globalization;

namespace Models.Billboards.Eventhall;

public class Service(IOptions<Options> options, 
    ITitleNormalization titleNormalizationService,
    ILogger<Service> logger) : PageParseService(options, titleNormalizationService, logger)
{
    public override BillboardTypes BillboardType => BillboardTypes.Eventhall;

    protected override List<DateTime>? GetEventDates(HtmlNode afishaItem,
        PageParseOptions options,
        string title = "")
    {
        try
        {
            var dates = new List<DateTime>();

            var dateItems = afishaItem.SelectNodes((options as Options).EventDiteTimeXPath);

            if (dateItems is null)
            {
                return dates;
            }

            for (int i = 0; i < dateItems.Count(); i += 3)
            {
                dates.Add(ParseDate($"{dateItems[i].InnerText.Trim().Replace(",", "")} {dateItems[i + 1].InnerText.Trim().Split(" ")[1]}",
                    options.DateFormats));
            }
            return dates;
        }
        catch (Exception exception)
        {
            LogHelper.LogInformation(logger,
                BillboardType,
                BillboardLoadingState.Processing,
                $"Fail parse items ({title} - {PageBlock.Date}): {exception.Message}");
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
            LogHelper.LogInformation(logger,
                BillboardType,
                BillboardLoadingState.Processing,
                $"Fail parse items ({title} - {PageBlock.Image}): {exception.Message}");
            return null;
        }
    }

    public override async Task<EventsResult> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes> searchEventTypes)
    {
        var options = (_options as Options);
        var result = new List<Event>();

        if (!searchEventTypes.Contains(EventTypes.Unidentified))
        {
            return new EventsResult();
        }

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

                var link = GetLink(afishaItem, options.LinkXPath, 
                    title: title);

                if (link is null)
                {
                    substandard = true;
                    link = options.BaseSearchUrl;
                }

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
            LogHelper.LogWarning(logger,
                BillboardType,
                BillboardLoadingState.Failed,
                $"Fail parse page: {exception.Message}");
        }

        return new EventsResult()
        {
            Result = [.. result
            .Where(r => !r.Substandard)
            .ToList()
            .DateGrouping()],

            SubstandardEvents = result
            .Where(r => r.Substandard)
            .ToList()
        };
    }

}
