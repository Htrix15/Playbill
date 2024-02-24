using Models.Billboards.Common.Options;

namespace Models.Billboards.Matreshkavrn;

public class Options : PageParseOptions
{
    public string? Place { get; set; }
    public string? EventInfoXPath { get; set; }
    public string? SingleDateFormat { get; set; }
    public string? SingleDateTimeFormat { get; set; }
}
