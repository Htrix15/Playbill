using Playbill.Billboards.Common.Enums;
using Playbill.Common.Event;

namespace Playbill.Billboards.Ya;

public class ConvertToEventSetting : BaseConvertToEventSetting
{
    public EventTypes EventType { get; set; }
    public string BaseLinkUrl { get; set; }
}