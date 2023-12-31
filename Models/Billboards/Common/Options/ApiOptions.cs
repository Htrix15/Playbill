﻿using Models.Billboards.Common.Enums;
using Models.Events;

namespace Models.Billboards.Common.Options;

public abstract class ApiOptions<TEventKeys> : BaseOptions
{
    public Dictionary<EventTypes, HashSet<TEventKeys>>? EventKeys { get; set; }
    public Dictionary<QueryKeys, string>? QueryKeys { get; set; }
    public Dictionary<QueryKeys, string>? QueryKeysConstants { get; set; }
    public int TimeOffset { get; set; }
}
