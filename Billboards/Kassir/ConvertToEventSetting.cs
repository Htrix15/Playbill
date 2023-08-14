using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Event;

namespace Playbill.Billboards.Kassir;

public class ConvertToEventSetting : BaseConvertToEventSetting
{
    public Dictionary<int, EventTypes> EventTypes { get; set; }
    public string? BasePathForLink { get; set; }
    public int TimeOffset { get; set; } = 0;
}
