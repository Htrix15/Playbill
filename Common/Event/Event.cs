﻿namespace Playbill.Common.Event;

public class Event
{
    public required BillboardTypes Billboard { get; set; }
    public required EventTypes Type { get; set; }
    public required List<DateTime>? Dates { get; set; }
    public DateTime? Date => Dates?.FirstOrDefault();
    public List<DateOnly>? EstimatedDates { get; set; }
    public required string? Title { get; set; }
    public required string? ImagePath { get; set; }
    public required string? Place { get; set; }
    public required List<EventLink> Links { get; set; }
}