using Models.Events;

namespace Models.Billboards.Common.Extension;

public static class EventTypesExtension
{
    public static Dictionary<EventTypes, HashSet<TEventKeys>> FilterEventKeys<TEventKeys>(this Dictionary<EventTypes, HashSet<TEventKeys>> eventKeys,
        HashSet<EventTypes>? searchEventTypes)
      => searchEventTypes is null || !searchEventTypes.Any()
          ? eventKeys
          : eventKeys.Where(eventKey => searchEventTypes.Contains(eventKey.Key))
                .ToDictionary(eventKey => eventKey.Key, eventKey => eventKey.Value);

}
