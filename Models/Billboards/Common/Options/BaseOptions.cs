
namespace Models.Billboards.Common.Options;

public abstract class BaseOptions
{
    public required string BaseSearchUrl { get; set; }
    public required string BaseLinkUrl { get; set; }
    public required int MaxDegreeOfParallelism { get; set; }
    /// <summary>
    /// In case of an error, stop requests for this time
    /// </summary>
    public required int TimeOut { get; set; }
    /// <summary>
    /// Number of attempts to make a request after an error
    /// </summary>
    public required int TryCount { get; set; }
    public string DateFormat { get; set; } = string.Empty;

}
