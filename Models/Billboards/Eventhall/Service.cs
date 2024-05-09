using HtmlAgilityPack;
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
                    var dates = new List<DateTime>();
                    var dateItems = afishaItem.SelectNodes(options.EventDiteTimeXPath);

                    if (dateItems is null)
                    {
                        continue;
                    }

                    for (int i = 0; i < dateItems.Count(); i += 3)
                    {
                        var time = DateTime.ParseExact(dateItems[i + 1].InnerText.Trim().Split(" ")[1], timeFormat, CultureInfo.CurrentCulture);
                        var date = DateTime.ParseExact(dateItems[i].InnerText.Trim().Replace(",", ""), dateFormat, CultureInfo.CurrentCulture).AddHours(time.Hour).AddMinutes(time.Minute);
                        dates.Add(date);
                    }

                    var eventType = EventTypes.Unidentified;

                    var nameItem = afishaItem.SelectSingleNode(options.EventTitleXPath);
                    var title = nameItem.InnerText.Trim()
                        .Replace("&quot;", "\"")
                        .Replace("&#171;", "«")
                        .Replace("&#187;", "»")
                        .Replace("&amp;", "&");
                    var link = afishaItem.SelectSingleNode("a").Attributes["href"].Value;

                    var imageItems = afishaItem.SelectNodes(options.EventImageXPath);

                    var imagePath = string.Empty;
                    foreach (var imageItem in imageItems)
                    {
                        var imageAttribute = imageItem.Attributes["data-original"];
                        if (imageAttribute == null) continue;
                        imagePath = imageAttribute.Value;
                    }

                    result.Add(new Event()
                    {
                        Billboard = BillboardType,
                        Type = eventType,
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
                    Console.WriteLine($"Fail parse items (Eventhall): {exception.Message}");
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse page (Eventhall): {exception.Message}");
        }

        result = result.DateGrouping().ToList();

        return result;
    }

}
