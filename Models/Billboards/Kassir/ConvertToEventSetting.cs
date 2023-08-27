using Models.Events;

namespace Models.Billboards.Kassir;

public class ConvertToEventSetting : BaseConvertToEventSetting
{
    public required Dictionary<int, EventTypes> EventTypes { get; set; }
    public required string? BasePathForLink { get; set; }
    public required int TimeOffset { get; set; } = 0;
    public DateOnly EstimatedDate { get; set; }
}
