
namespace Models.Billboards.Common.Options;

public abstract class BaseOptions
{
    public required string BaseSearchUrl { get; set; }
    public string BaseLinkUrl { get; set; }
    public int MaxDegreeOfParallelism { get; set; }
    /// <summary>
    /// In case of an error, stop requests for this time
    /// </summary>
    public int TimeOut { get; set; }
    /// <summary>
    /// Number of attempts to make a request after an error
    /// </summary>
    public int TryCount { get; set; }
    public string DateFormat { get; set; } = string.Empty;

}
