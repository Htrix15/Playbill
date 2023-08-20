using Playbill.Billboards.Common.Options;

namespace Playbill.Billboards.Eventhall;

public class Options : PageParseOptions
{
    public string? Place { get; set; }
    public string? TimeFormat { get; set; }
    public string? EventDiteTimeXPath { get; set; }
    public string? EventTimeXPath { get; set; }
}
