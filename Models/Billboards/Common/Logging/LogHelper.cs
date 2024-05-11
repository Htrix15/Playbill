using Microsoft.Extensions.Logging;
using Models.Billboards.Common.Enums;

namespace Models.Billboards.Common.Logging;

public static class LogHelper
{
    private static int ProgressStepLoging = 2;

    public static void LogProgressInformation<T>(ILogger<T> logger,
       BillboardTypes billboardType,
       int stepCount,
       int step,
       int? progressStepLoging = null)
    {
        if (step % (progressStepLoging ?? ProgressStepLoging) == 0)
        {
            logger.Log(
                LogLevel.Information,
                new EventId(),
                state: new LogInfo()
                {
                    BillboardType = billboardType,
                    State = BillboardLoadingState.Processing,
                    StepCount = stepCount,
                    Step = step
                },
                null,
                null);
        }
    }

    public static void LogInformation<T>(ILogger<T> logger, 
        BillboardTypes billboardType, 
        BillboardLoadingState state,
        string? message = null,
        int? stepCount = null,
        int? step = null)
    {
        logger.Log(
           LogLevel.Information,
           new EventId(),
           state: new LogInfo()
           {
               BillboardType = billboardType,
               State = state,
               Message = message,
               StepCount = stepCount,
               Step = step
           },
           null,
           null);
    }

    public static void LogWarning<T>(ILogger<T> logger, 
        BillboardTypes billboardType, 
        BillboardLoadingState state,
        string? message = null,
        int? stepCount = null,
        int? step = null)
    {
        logger.Log(
           LogLevel.Warning,
           new EventId(),
           state: new LogInfo()
           {
               BillboardType = billboardType,
               State = state,
               Message = message,
               StepCount = stepCount,
               Step = step
           },
           null,
           null);
    }
}
