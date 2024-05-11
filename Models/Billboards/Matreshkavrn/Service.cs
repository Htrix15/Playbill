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
using System.Text.RegularExpressions;

namespace Models.Billboards.Matreshkavrn;

public partial class Service(IOptions<Options> options,
    ITitleNormalization titleNormalizationService,
    ILogger<Service> logger) : PageParseService(options, titleNormalizationService, logger)
{
    public override BillboardTypes BillboardType => BillboardTypes.Matreshkavrn;

    protected override string? GetTitle(HtmlNode afishaItem, string eventInfoXPath)
    {
        try
        {
            var info = afishaItem.SelectSingleNode(eventInfoXPath);
            if (info is null) return null;
            var items = info.InnerHtml.Split("<br>");
            return items[0].Replace('\n', ' ').Trim();
        }
        catch (Exception exception)
        {
            LogHelper.LogInformation(logger,
                BillboardType,
                BillboardLoadingState.Processing,
                $"Fail parse items ({PageBlock.Title}): {exception.Message}");
            return null;
        }
    }

    protected override List<DateTime>? GetEventDates(HtmlNode afishaItem,
        PageParseOptions options,
        string title)
    {
        try
        {
            var dates = new List<DateTime>();

            var singleDateFormat = (options as Options).SingleDateFormat;
            var singleDateTimeFormat = (options as Options).SingleDateTimeFormat;

            var info = afishaItem.SelectSingleNode((options as Options).EventInfoXPath);

            if (info is null) return dates;

            var items = info.InnerHtml.Split("<br>");
            var dateStr = items[1]
                    .Replace('\n', ' ')
                    .Replace("  ", " ")
                    .Replace("<strong>", "")
                    .Replace("</strong>", "")
                    .Trim();
       
            if (TimeRange().IsMatch(dateStr))
            {
                var match = TimeRange().Match(dateStr);
                dateStr = dateStr.Remove(match.Index).Trim();
            }

            if (dateStr.Contains('-'))
            {
                var dateRange = dateStr.Split(' ')[0].Replace("  ", " ").Trim();
                var dateMonth = dateStr.Split(' ')[1].Replace("  ", " ").Trim();
                var startDay = int.Parse(dateRange.Split('-')[0].Replace("  ", " ").Trim());
                var endDay = int.Parse(dateRange.Split('-')[1].Replace("  ", " ").Trim());
                for (var i = startDay; i <= endDay; i++)
                {
                    dates.Add(DateTime.ParseExact($"{i} {dateMonth}", singleDateFormat, CultureInfo.CurrentCulture));
                }
            }
            else if (dateStr.Contains(','))
            {
                var days = dateStr.Split(',');
                var dayWithMonth = days.Last().Split(' ');
                var month = dayWithMonth.Last();
                days[days.Length - 1] = dayWithMonth.First();
                foreach (var day in days)
                {
                    dates.Add(DateTime.ParseExact($"{day} {month}", singleDateFormat, CultureInfo.CurrentCulture));
                }
            }
            else
            {
                try
                {
                    dates.Add(DateTime.ParseExact(dateStr, singleDateTimeFormat, CultureInfo.CurrentCulture));
                }
                catch
                {
                    dates.Add(DateTime.ParseExact(dateStr, singleDateFormat, CultureInfo.CurrentCulture));
                }
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

    protected string? GetPlace(Options options)
    {
        return options.Place;
    }

    public override async Task<EventsResult> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes> searchEventTypes)
    {
        var options = _options as Options;
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
                var title =  GetTitle(afishaItem, options.EventInfoXPath);
                if (title is null) continue;

                var imagePath = GetImagePath(afishaItem, options.EventImageXPath, title: title);

                var dates = GetEventDates(afishaItem, options, title: title);
                var substandard = dates is null;
                dates = FilterDate(dates, eventDateIntervals);
                if (!substandard && dates.Count == 0) continue;

                var place = GetPlace(options);
                var link = GetLink(afishaItem, 
                    options.LinkXPath, 
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

    [GeneratedRegex("\\d{1,2}:\\d{2}.{1,}\\d{1,2}:\\d{2}")]
    private static partial Regex TimeRange();
}
