using Playbill.Billboards.Common.Enums;

namespace Playbill.Billboards.Common.Extension;

public static class EventTypesExtension
{
    public static Dictionary<EventTypes, HashSet<TEventKeys>> FilterEventKeys<TEventKeys>(this Dictionary<EventTypes, HashSet<TEventKeys>> eventKeys, 
        IList<EventTypes>? searchEventTypes)
      => searchEventTypes is null
          ? eventKeys
          : eventKeys.Where(eventKey => searchEventTypes.Contains(eventKey.Key))
                .ToDictionary(eventKey => eventKey.Key, eventKey => eventKey.Value);

}
