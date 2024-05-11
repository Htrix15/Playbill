using Models.Billboards.Common.Enums;

namespace Models.Billboards.Common.Logging;

public class LogInfo
{
    public BillboardTypes BillboardType { get; set; }
    public BillboardLoadingState State { get; set; }
    /// <summary>
    /// For BillboardLoadingState.Processing
    /// </summary>
    public int? StepCount { get; set; }
    /// <summary>
    /// For BillboardLoadingState.Processing
    /// </summary>
    public int? Step { get; set; }
    public string? Message { get; set; }
}

public enum BillboardLoadingState
{
    Started,
    End,
    Processing,
    Failed
}