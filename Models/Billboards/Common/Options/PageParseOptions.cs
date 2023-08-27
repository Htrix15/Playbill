using Models.Events;

namespace Models.Billboards.Common.Options;

public abstract class PageParseOptions: BaseOptions
{
    public required string ItemsContainerXPath { get; set; }
    public string? ItemsXPath { get; set; }
    public string? EventTitleXPath { get; set; }
    public string? EventDateXPath { get; set; }
    public string? EventImageXPath { get; set; }
    public string? PlaceXPath { get; set; }
    public string? LinkXPath { get; set; }
    public required Dictionary<EventTypes, HashSet<string>>? EventKeys { get; set; }
}
