using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Users;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBot;
using TelegramBot.Handles;
using TelegramBot.Services;

var customServices = new List<Action<ServiceCollection>>()
{
    services => services.AddScoped<IUpdateHandler, UpdateHandler>(),
    services => services.AddScoped<ReceiverService>(),
    services => services.AddSingleton(new ReceiverOptions()
    {
        AllowedUpdates = Array.Empty<UpdateType>(),
        ThrowPendingUpdates = true,
    }),
    services => services.AddScoped<MessageService>(),
    services => services.AddScoped<MarkupService>(),
    services => services.AddScoped<EventService>(),
    services => services.AddScoped<UserSettingsService>()
};

var customConfigurations = new List<(string File, bool Optional)>()
{
    ("bot-configuration.json", false)
};

var customOptions = new List<Action<ServiceCollection, IConfigurationRoot>>()
{
    (services, configuration) => services.Configure<BotConfiguration>(configuration.GetSection("BotConfiguration")),
};


var customServicesWithOptions = new List<Action<ServiceCollection, IConfigurationRoot>>()
{
    (services, configuration) => {
        var token = configuration.GetValue<string>("BotConfiguration:Token") ?? throw new Exception("Bot token not found"); 
        var botClient = new TelegramBotClient(token);
        services.AddSingleton<ITelegramBotClient>(botClient);
    }
};

var customDb = new List<Action<ServiceCollection>>()
{
    services => services.AddTransient<IUserSettingsRepository, UserSettingsRepository>()
};

using var serviceProvider = Builder
    .GetServiceCollection(
        customServices: customServices,
        customConfigurations: customConfigurations,
        customOptions: customOptions,
        customServicesWithOptions: customServicesWithOptions,
        customDb: customDb
    )
    .BuildServiceProvider();

await serviceProvider.GetService<ReceiverService>()!.ReceiveAsync();

