using Models.Billboards;

namespace Models.Events;

public class EventLink
{
    public required BillboardTypes BillboardType { get; set; }
    public required string? Path { get; set; }
}
