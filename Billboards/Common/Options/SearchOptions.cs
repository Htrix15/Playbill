using Playbill.Common;
using Playbill.Common.Event;

namespace Playbill.Billboards.Common.Options;

public class SearchOptions
{
    public HashSet<BillboardTypes>? SupportedBillboards { get; set; }
    public HashSet<EventTypes>? SearchEventTypes { get; set; }
    public bool RemoveUnidentifiedEventType { get; set; }
}
