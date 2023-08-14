using Playbill.Billboards.Common.Enums;
using Playbill.Common;

namespace Playbill.Billboards.Common.Event;

public class Event
{
    public required BillboardTypes Billboard { get; set; }
    public required EventTypes Type { get; set; }
    public required DateTime? Date { get; set; }
    public bool MultipleTime { get; set; }
    public required string? Title { get; set; }
    public required string? ImagePath { get; set; }
    public required string? Place { get; set; }
    public required List<EventLink> Links { get; set; }
}
