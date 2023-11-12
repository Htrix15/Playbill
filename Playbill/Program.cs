using Microsoft.Extensions.DependencyInjection;
using Models.Search;
using Models.ProcessingServices;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using ConsoleApp.ExportToHtml;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;

var customServices = new List<Action<ServiceCollection>>()
{
     services => services.AddTransient<ExportToHtmlService>()
};

var customConfigurations = new List<(string File, bool Optional)>()
{
    ("export-to-html-settings.json", false)
};

var customOptions = new List<Action<ServiceCollection, IConfigurationRoot>>()
{
    (services, configuration) => services.Configure<ExportToHtmlOptions>(configuration.GetSection("ExportToHtml"))
};

using var serviceProvider = Builder
    .GetServiceCollection(
        customServices: customServices,
        customConfigurations: customConfigurations,
        customOptions: customOptions
    )
    .BuildServiceProvider();

try
{
    Console.WriteLine("Хай! Я ищу афишу для Воронежа.");
    Console.WriteLine("Настроить куда выкладывать получившийся файл можно в конфиге: export-to-html-settings.json");
    Console.WriteLine("Настроить ограничения по поиску можно в конфиге: search-options.json");
    Console.WriteLine();
    Console.WriteLine("За какой период искать? Надо ввести соответсвующую цифру:");
    Console.WriteLine("На этой неделе".PadRight(27) + $"{(int)DatePeriods.ThisWeek}");
    Console.WriteLine("На следующей неделе".PadRight(27) + $"{(int)DatePeriods.NextWeek}");
    Console.WriteLine("На этой и следующей неделе".PadRight(27) + $"{(int)DatePeriods.ThisAndNextWeek}");
    Console.WriteLine("До конца месяца".PadRight(27) + $"{(int)DatePeriods.ThisMonth}");
    Console.WriteLine("Ближайшие 30 дней".PadRight(27) + $"{(int)DatePeriods.Next30Days}");
    Console.WriteLine("Ближайшие 60 дней".PadRight(27) + $"{(int)DatePeriods.Next60Days}");
    Console.WriteLine("До конца года".PadRight(27) + $"{(int)DatePeriods.ThisYear}");

    DatePeriods datePeriod;

    do
    {
        Console.Write("> ");
        var value = Console.ReadLine();
        if (int.TryParse(value, out var number))
        {
            if (Enum.IsDefined(typeof(DatePeriods), number))
            {
                datePeriod = (DatePeriods)number;
                break;
            }
        }
        Console.WriteLine("Неправильно, еще раз");
    } while (true);

    Console.WriteLine("Начат поиск...");
    var events = await serviceProvider.GetService<MainService>()!.GetEvents(new SearchOptions() { DatePeriod = datePeriod }, true);
    await serviceProvider.GetService<ExportToHtmlService>()!.ExportAync(events);
    Console.WriteLine("Готово, можно закрыть!");
}
catch(Exception ex)
{
    Console.WriteLine("Сломалось, можно закрыть или почитать об ошибке :(");
    Console.WriteLine("Message: " + ex.Message);
    Console.WriteLine("StackTrace: " + ex.StackTrace);
    Console.WriteLine("InnerException: " + ex.InnerException);
}
Console.ReadLine();
