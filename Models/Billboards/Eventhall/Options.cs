using Models.Billboards.Common.Options;

namespace Models.Billboards.Eventhall;

public class Options : PageParseOptions
{
    public string? Place { get; set; }
    public string? TimeFormat { get; set; }
    public string? EventTimeXPath { get; set; }
}
