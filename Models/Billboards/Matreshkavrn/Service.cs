using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Service;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Models.Billboards.Matreshkavrn;

public partial class Service : PageParseService
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    public override BillboardTypes BillboardType => BillboardTypes.Matreshkavrn;

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes> searchEventTypes)
    {
        var options = _options as Options;

        var baseSearchUrl = options.BaseSearchUrl;
        var place = options.Place;
        var singleDateFormat = options.SingleDateFormat;
        var singleDateTimeFormat = options.SingleDateTimeFormat;

        var result = new List<Event>();

        try
        {
            var web = new HtmlWeb();

            var doc = await web.LoadFromWebAsync(baseSearchUrl);

            var afishaItems = doc.DocumentNode.SelectNodes(options.ItemsXPath);
            foreach (var afishaItem in afishaItems)
            {
                try
                {
                    var info = afishaItem.SelectSingleNode(options.EventInfoXPath);

                    if (info is null) continue;
                    var items = info.InnerHtml.Split("<br>");
                    var title = items[0].Replace('\n',' ').Trim();
                    var dateStr = items[1]
                        .Replace('\n', ' ')
                        .Replace("  "," ")
                        .Replace("<strong>", "")
                        .Replace("</strong>", "")
                        .Trim();
                    var dates = new List<DateTime>();

                    if (TimeRange().IsMatch(dateStr))
                    {
                        var match = TimeRange().Match(dateStr);
                        dateStr = dateStr.Remove(match.Index).Trim();
                    }

                    if (!dateStr.Contains('-'))
                    {
                        try
                        {
                            dates.Add(DateTime.ParseExact(dateStr, singleDateTimeFormat, CultureInfo.CurrentCulture));
                        }
                        catch
                        {
                            try
                            {
                                dates.Add(DateTime.ParseExact(dateStr, singleDateFormat, CultureInfo.CurrentCulture));
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine($"Fail parse date (Matreshkavrn, {dateStr}): {exception.Message}");
                            }
                        }
                    }
                    else
                    {
                        try
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
                        catch (Exception exception)
                        {
                            Console.WriteLine($"Fail parse date (Matreshkavrn, {dateStr}): {exception.Message}");
                        }
                    }

                    var imageItem = afishaItem.SelectSingleNode(options.EventImageXPath);
                    var imagePath = "";
                    if (imageItem is not null)
                    {
                        imagePath = imageItem.Attributes["src"].Value;
                    }

                    var linkItem = afishaItem.SelectSingleNode(options.LinkXPath);
                    var link = "";
                    if (linkItem is not null)
                    {
                      link = linkItem.Attributes["href"].Value;
                    } else
                    {
                        link = baseSearchUrl;
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
                            }
                    });
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Fail parse items (Matreshkavrn): {exception.Message}");
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse page (Matreshkavrn): {exception.Message}");
        }

        result = result.DateGrouping().ToList();

        return result;
    }

    [GeneratedRegex("\\d{1,2}:\\d{2}.{1,}\\d{1,2}:\\d{2}")]
    private static partial Regex TimeRange();
}
