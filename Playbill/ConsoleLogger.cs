using Microsoft.Extensions.Logging;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Logging;

namespace ConsoleApp;

public class ConsoleLogger() : ILogger, IDisposable
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return this;
    }

    public void Dispose() { }

    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    static private Dictionary<BillboardTypes, LogInfo> _billboardStates = new();

    private void ChangeConsoleForegroundColor(BillboardLoadingState state)
    {
        var defaultColor = Console.ForegroundColor;

        Console.ForegroundColor = state switch
        {
            BillboardLoadingState.Started => ConsoleColor.Blue,
            BillboardLoadingState.End => ConsoleColor.Green,
            BillboardLoadingState.Processing => ConsoleColor.Blue,
            BillboardLoadingState.Failed => ConsoleColor.Red,
            _ => defaultColor
        };
    }

    private void RefreashBillbordLoadingInfo(LogInfo logInfo)
    {
        if (_billboardStates.TryGetValue(logInfo.BillboardType, out var _))
        {
            _billboardStates[logInfo.BillboardType].State = logInfo.State;
            _billboardStates[logInfo.BillboardType].Step = logInfo.Step;
            _billboardStates[logInfo.BillboardType].StepCount = logInfo.StepCount;
            if (!string.IsNullOrEmpty(logInfo.Message))
            {
                string[] messages = [_billboardStates[logInfo.BillboardType].Message, logInfo.Message];
                _billboardStates[logInfo.BillboardType].Message = string.Join(", ", messages.Where(m => !string.IsNullOrEmpty(m)));
            }
        }
        else
        {
            _billboardStates.TryAdd(logInfo.BillboardType, logInfo);
        }

        const int leftPad = 12;

        Console.Clear();
        var orderedStates = _billboardStates.OrderBy(k => k.Key);
        foreach (var billboardState in orderedStates)
        {
            var defaultColor = Console.ForegroundColor;
            Console.Write($"{billboardState.Key, leftPad}: ");
            ChangeConsoleForegroundColor(billboardState.Value.State);
            Console.Write(billboardState.Value.State);
            if (billboardState.Value.State == BillboardLoadingState.Processing
                && billboardState.Value.StepCount != null
                && billboardState.Value.Step != null)
            {
                Console.Write($" - {billboardState.Value.Step:d3}\\{billboardState.Value.StepCount:d3}");
            }
            Console.WriteLine();
            Console.ForegroundColor = defaultColor;
        }
        var stateWithMessages = orderedStates.Where(s => !string.IsNullOrEmpty(s.Value.Message));

        if (stateWithMessages.Any())
        {
            Console.Write("\nMessages\n\n");
            foreach (var billboardState in stateWithMessages)
            {
                var defaultColor = Console.ForegroundColor;
                Console.Write($"{billboardState.Key, leftPad}: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(billboardState.Value.Message);
                Console.WriteLine();
                Console.ForegroundColor = defaultColor;
            }
        }
        Task.Delay(500).Wait();
    }


    static readonly object _lock = new ();
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {

        lock (_lock)
        {
            if (state is LogInfo)
            {
                RefreashBillbordLoadingInfo(state as LogInfo);
            }

            var defaultColor = Console.ForegroundColor;

            //Console.ForegroundColor = logLevel switch
            //{
            //    LogLevel.Information => ConsoleColor.Green,
            //    LogLevel.Warning => ConsoleColor.Yellow,
            //    LogLevel.Error => ConsoleColor.Red,
            //    _ => defaultColor
            //};

            //Console.WriteLine(state);

            //Console.ForegroundColor = defaultColor;
        }
    }
}

public class ConsoleLoggerProvider() : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new ConsoleLogger();
    }

    public void Dispose() {}
}
