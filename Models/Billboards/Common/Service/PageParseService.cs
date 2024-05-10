﻿using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Exceptions;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Options;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;

namespace Models.Billboards.Common.Service;

public abstract class PageParseService : BaseBillboardService
{
    protected PageParseService(IOptions<BaseOptions> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    protected List<KeyValuePair<EventTypes, HashSet<string>>> EventKeys(HashSet<EventTypes>? searchEventTypes) =>
    (_options as PageParseOptions)?.EventKeys?.FilterEventKeys(searchEventTypes).ToList() ?? new List<KeyValuePair<EventTypes, HashSet<string>>>();

    protected async Task<HtmlDocument> GetBuilbordPage(string url)
    {
        try
        {
            var page = await new HtmlWeb().LoadFromWebAsync(url);
            return page ?? throw new Exception();
        }
        catch
        {
            throw new FailParsePageExceptions(PageBlock.FullAfishaBlock);
        }
    }
    protected HtmlNode? GetFullAfisha(HtmlDocument doc, string itemsContainerXPath)
    {
        try
        {
            var fullAfisha = doc.DocumentNode.SelectSingleNode(itemsContainerXPath);
            return fullAfisha ?? throw new Exception();
        }
        catch
        {
            throw new FailParsePageExceptions(PageBlock.FullAfishaBlock);
        }

    }
    protected HtmlNodeCollection? GetAfishaItems(HtmlNode fullAfisha, string itemsXPath)
    {
        try
        {
            var items = fullAfisha.SelectNodes(itemsXPath);
            return items ?? throw new Exception();
        }
        catch
        {
            throw new FailParsePageExceptions(PageBlock.AfishaItems);
        }
    }

    protected abstract List<DateTime>? GetEventDates(HtmlNode afishaItem,
        PageParseOptions options,
        string title = "");

    protected virtual string? GetTitle(HtmlNode afishaItem, string eventTitleXPath)
    {
        try
        {
            var nameItem = afishaItem.SelectSingleNode(eventTitleXPath);
            return nameItem.InnerText
                .Trim()
                .Replace("&quot;", "\"")
                .Replace("&#171;", "«")
                .Replace("&#187;", "»")
                .Replace("&amp;", "&");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {PageBlock.Title}): {exception.Message}");
            return null;
        }
    }

    protected virtual string? GetPlace(HtmlNode afishaItem, 
        string placeXPath,
        string title = "")
    {
        try
        {
            var placeItem = afishaItem.SelectSingleNode(placeXPath);
            return placeItem.InnerText.Trim();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {title} - {PageBlock.Place}): {exception.Message}");
            return null;
        }
    }

    protected virtual string? GetImagePath(HtmlNode afishaItem, 
        string eventImageXPath,
        string baseLinkUrl = "",
        string title = "")
    {
        try
        {
            var imageItem = afishaItem.SelectSingleNode(eventImageXPath);
            return baseLinkUrl + imageItem.Attributes["src"].Value;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {title} - {PageBlock.Image}): {exception.Message}");
            return null;
        }
    }

    protected virtual string? GetLink(HtmlNode afishaItem,
        string linkXPath,
        string baseLinkUrl = "", 
        string title = "")
    {
        try
        {
            var linkItem = afishaItem.SelectSingleNode(linkXPath);
            return baseLinkUrl + linkItem.Attributes["href"].Value;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail parse items ({BillboardType} - {title} - {PageBlock.Link}): {exception.Message}");
            return baseLinkUrl;
        }
    }

    protected List<DateTime>? FilterDate(List<DateTime>? eventDates, IList<EventDateInterval>? dateRanges)
    {
        if (eventDates is null || dateRanges is null) return eventDates;
        return eventDates.Where(d =>
        {
            var chackDate = new DateOnly(d.Year, d.Month, d.Day);
            return d > DateTime.Now && dateRanges.Any(eventDateInterval => chackDate >= eventDateInterval.StartDate && chackDate <= eventDateInterval.EndDate);
        }).ToList();
    }

 
}
