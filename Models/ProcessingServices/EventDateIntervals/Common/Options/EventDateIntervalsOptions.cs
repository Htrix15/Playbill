
namespace Models.ProcessingServices.EventDateIntervals.Common.Options;

public class EventDateIntervalsOptions
{
    public string PathYearTerm { get; set; }
    public string Path { get; set; }
    public string Request => Path.Replace(PathYearTerm, DateTime.Now.Year.ToString());
    public HashSet<DateOnly> MainHolidays { get; set; }
}
