using Models.ProcessingServices.EventDateIntervals.Common.Enums;

namespace TelegramBot.Params;

public class GetEventsParams : BaseParams
{
    public required long UserId { get; set; }
    public required DatePeriods DatePeriod { get; set; }
}
