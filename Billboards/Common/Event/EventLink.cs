using Playbill.Common;

namespace Playbill.Billboards.Common.Event;

public class EventLink
{
    public required BillboardTypes BillboardType { get; set; }
    public required string Path { get; set; }
}
