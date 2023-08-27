using Models.Events;

namespace Models.Billboards.Ya;

public class ConvertToEventSetting : BaseConvertToEventSetting
{
    public EventTypes EventType { get; set; }
    public string BaseLinkUrl { get; set; }
}