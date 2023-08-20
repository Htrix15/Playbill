﻿using Playbill.Common.Event;

namespace Playbill.Billboards.Common.Extension;

public static class TurnDictinoryExtension
{
    public static Dictionary<TEventKeys, EventTypes> Flip<TEventKeys>(this Dictionary<EventTypes, HashSet<TEventKeys>> dictionary)
        => dictionary
            .SelectMany(item => item.Value.Select(categoryId => (categoryId, eventType: item.Key)))
            .ToDictionary(key => key.categoryId, key => key.eventType);

}
