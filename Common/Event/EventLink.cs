using Playbill.Common;

namespace Playbill.Common.Event;

public class EventLink
{
    public required BillboardTypes BillboardType { get; set; }
    public required string? Path { get; set; }
}
