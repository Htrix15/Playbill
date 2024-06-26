﻿using Models.Billboards.Common.Enums;

namespace Models.Events;

public class Event
{
    public required BillboardTypes Billboard { get; set; }
    public required EventTypes Type { get; set; }
    public required List<DateTime>? Dates { get; set; }
    public DateTime? Date => Dates?.FirstOrDefault();
    public List<TimeOnly>? Sessions { get; set; }
    public List<DateOnly>? EstimatedDates { get; set; }
    public required string? Title { get; set; }
    public string NormilizeTitle { get; set; }
    public List<string> NormilizeTitleTerms { get; set; }
    public required string? ImagePath { get; set; }
    public required string? Place { get; set; }
    public bool SplitPlace { get; set; }
    public required List<EventLink> Links { get; set; }
    public bool Substandard { get; set; }
    public bool SubstandardByDate => Substandard && Date is null;
}
