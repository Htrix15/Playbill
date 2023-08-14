using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Event;
using Playbill.Billboards.Common.Interfaces;

namespace Playbill.Billboards.Kassir;

public class Response: IConvertToEvent<int>
{
    public required PaginationOptions Pagination { get; set; }
    public required List<Item> Items { get; set; }

    public (IList<Event> events, IList<Event> failedEvents) ConvertToEvents(BaseConvertToEventSetting convertToEventSetting)
    {
        var setting = convertToEventSetting as ConvertToEventSetting;
        if (!Items.Any()) return (new List<Event>(), new List<Event>());

        var resultEvent = new List<Event>();
        var resultFailedEvent = new List<Event>();
        foreach (var item in Items)
        {
            try
            {
                var eventType = EventTypes.FailedEvent;
                var categoryId = (item.Object?.Category?.Id ?? item.Object?.Activity?.Category?.Id);
                if (categoryId != null && (setting?.EventTypes.TryGetValue(categoryId.Value, out var newEventType) ?? false))
                {
                    eventType = newEventType;
                }

                var date = (item.Object?.BeginsAt ?? item.Object?.DateRange?.BeginsAt)?.AddHours(setting?.TimeOffset ?? 0);
                var place = item.Object?.Venues?.FirstOrDefault()?.Name ?? item.Object?.Hall?.Venue?.Name;
                var title = item.Object?.Title;
                var imagePath = item.Object?.PosterImage;
                var path = item.Object?.UrlSlug is not null ? setting?.BasePathForLink + "/" + item.Object.UrlSlug : null;

                if (date is null || place is null || title is null || imagePath is null || path is null)
                {
                    eventType = EventTypes.FailedEvent;
                }

                var @event = new Event()
                {
                    Billboard = Playbill.Common.BillboardTypes.Kassir,
                    Type = eventType,
                    Date = date,
                    Title = title,
                    ImagePath = imagePath,
                    Place = place,
                    Links = new List<EventLink>()
                    {
                        new EventLink()
                        {
                            BillboardType = Playbill.Common.BillboardTypes.Kassir,
                            Path = path
                        }
                    }
                };
                if (@event.Type != EventTypes.FailedEvent)
                {
                    resultEvent.Add(@event);
                } else
                {
                    resultFailedEvent.Add(@event);
                }
              
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Fail convert: {item.Object?.Title ?? "?"} Message : {exception.Message}");
            }
        }
        return (resultEvent, resultFailedEvent);
    }

    public class Activity
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public object? TechDescription { get; set; }
        public string? Slug { get; set; }
        public Category? Category { get; set; }
        public List<ExtraCategory>? ExtraCategories { get; set; }
        public bool? HasEqualEvents { get; set; }
        public string? PosterImage { get; set; }
        public object? DateRange { get; set; }
        public object? TimeRange { get; set; }
        public object? EventDurationRange { get; set; }
        public PriceRange? PriceRange { get; set; }
        public List<object>? AgeGroups { get; set; }
        public bool? IsPushkin { get; set; }
        public string? UrlSlug { get; set; }
    }

    public class Address
    {
        public object? City { get; set; }
        public string? Lng { get; set; }
        public string? Lat { get; set; }
        public List<object>? Subways { get; set; }
        public string? AddressString { get; set; }
        public bool? IsNormalized { get; set; }
    }

    public class AgeGroup
    {
        public string? Name { get; set; }
    }

    public class Category
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public object? Parent { get; set; }
        public List<Child>? Children { get; set; }
        public string? Slug { get; set; }
        public string? SlugForEvent { get; set; }
    }

    public class Child
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public object? Parent { get; set; }
        public object? Children { get; set; }
        public string? Slug { get; set; }
        public string? SlugForEvent { get; set; }
    }

    public class DateRange
    {
        public DateTime? BeginsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }

    public class ExtraCategory
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public Parent? Parent { get; set; }
        public List<Child>? Children { get; set; }
        public string? Slug { get; set; }
        public string? SlugForEvent { get; set; }
    }

    public class Hall
    {
        public int? Id { get; set; }
        public Venue? Venue { get; set; }
    }

    public class Item
    {
        public EventBody? Object { get; set; }
        public object? Advertising { get; set; }
        public string? Type { get; set; }
    }

    public class EventBody
    {
        public int? Id { get; set; }
        public int? BackofficeId { get; set; }
        public Activity? Activity { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public AgeGroup? AgeGroup { get; set; }
        public DateTime? BeginsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public string? PosterImage { get; set; }
        public bool? IsPushkin { get; set; }
        public PriceRange? PriceRange { get; set; }
        public Hall? Hall { get; set; }
        public string? UrlSlug { get; set; }
        public bool? IsMultiDay { get; set; }
        public string? Description { get; set; }
        public string? TechDescription { get; set; }
        public Category? Category { get; set; }
        public List<ExtraCategory>? ExtraCategories { get; set; }
        public bool? HasEqualEvents { get; set; }
        public DateRange? DateRange { get; set; }
        public object? TimeRange { get; set; }
        public object? EventDurationRange { get; set; }
        public List<AgeGroup>? AgeGroups { get; set; }
        public int? ActiveEventsCount { get; set; }
        public List<Venue>? Venues { get; set; }
    }

    public class PaginationOptions
    {
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? CurrentPage { get; set; }
        public int? Offset { get; set; }
        public int? PagesCount { get; set; }
    }

    public class Parent
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public object? parent { get; set; }
        public List<Child>? Children { get; set; }
        public string? Slug { get; set; }
        public string? SlugForEvent { get; set; }
    }

    public class PriceRange
    {
        public object? Min { get; set; }
        public object? Max { get; set; }
    }

    public class Venue
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public Address? Address { get; set; }
        public string? Uri { get; set; }
    }


}
