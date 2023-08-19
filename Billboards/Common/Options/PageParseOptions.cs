using Playbill.Billboards.Common.Enums;

namespace Playbill.Billboards.Common.Options;

public abstract class PageParseOptions: BaseOptions
{
    public required string ItemsContainerXPath { get; set; }
    public string? ItemsXPath { get; set; }
    public string? EventTitleXPath { get; set; }
    public string? EventDateXPath { get; set; }
    public string? EventImageXPath { get; set; }
    public string? PlaceXPath { get; set; }
    public string? LinkXPath { get; set; }
    public required Dictionary<EventTypes, string?>? EventKeys { get; set; }
}
