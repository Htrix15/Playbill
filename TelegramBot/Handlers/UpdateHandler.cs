using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using TelegramBot.Services;
using TelegramBot.Params;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;

namespace TelegramBot.Handles;

public class UpdateHandler : IUpdateHandler
{
    private readonly MessageService _messageService;
    private readonly UserSettingsService _userSettingsService;
    private readonly EventService _eventService;
    private readonly Dictionary<string, Func<BaseParams, Task>> _messageActions;
    private readonly Dictionary<string, Func<UpdateUserSettingsParams, Task>> _updateSettingsActions;

    private readonly List<string> _getEventsKeys = new List<string>()
    {
         Commands.ThisWeek, Commands.NextWeek, Commands.ThisMonth, Commands.Next30Days, Commands.ThisYear,
    };
    private readonly List<string> _settingsKeys = new List<string>()
    {
         Commands.Billboards, Commands.EventTypes, Commands.DaysOfWeek, Commands.Places, Commands.AddHolidays,
    };

    private Func<BaseParams, GetEventsParams, Task> GetEvents => async (BaseParams messageParams, GetEventsParams getEventsParams) =>
    {
        await _messageService.GetStartSearchMessageAsync(new MessageParams()
        {
            ChatId = messageParams.ChatId
        });

        var events = await _eventService.GetEvents(getEventsParams);
        var eventsMessagesParams = new EventsMessagesParams()
        {
            ChatId = messageParams.ChatId,
            Events = events
        };
        await _messageService.GetEventMessagesAsync(eventsMessagesParams);
    };

    public UpdateHandler(MessageService messageService, EventService eventService, UserSettingsService userSettingsService)
    {
        _messageService = messageService;
        _eventService = eventService;
        _userSettingsService = userSettingsService;
        _messageActions = new Dictionary<string, Func<BaseParams, Task>>()
        {
            { 
                Commands.Start, 
                messageParams => _messageService.GetStartMessageAsync(messageParams as MessageParams) 
            },
            {
                Commands.Search,
                messageParams => _messageService.GetSearchDatePeriodsMessageAsync(messageParams as MessageParams)
            },
            {
                Commands.Settings,
                messageParams => _messageService.GetSettingsMessageAsync(messageParams as MessageParams)
            },
            {
                Commands.Billboards,
                async messageParams => {
                    var userSettingsParams = messageParams as UserSettingsParams;
                    var settings = await _userSettingsService.GetUserBillboardsSettingsAsync(userSettingsParams.UserId);
                    userSettingsParams.Settings = settings;
                    await _messageService.GetSettingsListMessageAsync(userSettingsParams, $"{Commands.Billboards.TrimStart('/')}_");
                }
            },
            {
                Commands.EventTypes,
                async messageParams => {
                    var userSettingsParams = messageParams as UserSettingsParams;
                    var settings = await _userSettingsService.GetUserEventTypesSettingsAsync(userSettingsParams.UserId);
                    userSettingsParams.Settings = settings;
                    await _messageService.GetSettingsListMessageAsync(userSettingsParams,  $"{Commands.EventTypes.TrimStart('/')}_");
                }
            },
            {
                Commands.DaysOfWeek,
                async messageParams => {
                    var userSettingsParams = messageParams as UserSettingsParams;
                    var settings = await _userSettingsService.GetUserDaysOfWeekSettingsAsync(userSettingsParams.UserId);
                    userSettingsParams.Settings = settings;
                    await _messageService.GetSettingsListMessageAsync(userSettingsParams,  $"{Commands.DaysOfWeek.TrimStart('/')}_");
                }
            },
            {
                Commands.AddHolidays,
                async messageParams => {
                    var userSettingsParams = messageParams as UserSettingsParams;
                    var settings = await _userSettingsService.GetUserAddHolidaysSettingsAsync(userSettingsParams.UserId);
                    userSettingsParams.Settings = settings;
                    await _messageService.GetSettingsListMessageAsync(userSettingsParams,  $"{Commands.AddHolidays.TrimStart('/')}_");
                }
            },
            {
                Commands.ThisWeek, 
                async messageParams => {
                    var getEventsParams = messageParams as GetEventsParams;
                    getEventsParams.DatePeriod = DatePeriods.ThisWeek;
                    await GetEvents(messageParams, getEventsParams);
                } 
            },
            {
                Commands.NextWeek,
                async messageParams => {
                    var getEventsParams = messageParams as GetEventsParams;
                    getEventsParams.DatePeriod = DatePeriods.NextWeek;
                    await GetEvents(messageParams, getEventsParams);
                }
            },
            {
                Commands.ThisMonth,
                async messageParams => {
                    var getEventsParams = messageParams as GetEventsParams;
                    getEventsParams.DatePeriod = DatePeriods.ThisMonth;
                    await GetEvents(messageParams, getEventsParams);
                }
            },
            {
                Commands.Next30Days,
                async messageParams => {
                    var getEventsParams = messageParams as GetEventsParams;
                    getEventsParams.DatePeriod = DatePeriods.Next30Days;
                    await GetEvents(messageParams, getEventsParams);
                }
            },
            {
                Commands.ThisYear,
                async messageParams => {
                    var getEventsParams = messageParams as GetEventsParams;
                    getEventsParams.DatePeriod = DatePeriods.ThisYear;
                    await GetEvents(messageParams, getEventsParams);
                }
            }
        };
        _updateSettingsActions = new Dictionary<string, Func<UpdateUserSettingsParams, Task>> { 
            {
                Commands.Billboards.TrimStart('/'),  
                async settings =>
                {
                    await _userSettingsService.UpdateBillboardsSettingsAsync(settings);
                    await _messageService.UpdateSettingsListMessageAsync(settings);
                }
            },
            {
                Commands.EventTypes.TrimStart('/'),
                async settings =>
                {
                    await _userSettingsService.UpdateEventTypesSettingsAsync(settings);
                    await _messageService.UpdateSettingsListMessageAsync(settings);
                }
            },
            {
                Commands.DaysOfWeek.TrimStart('/'),
                async settings =>
                {
                    await _userSettingsService.UpdateDaysOfWeekSettingsAsync(settings);
                    await _messageService.UpdateSettingsListMessageAsync(settings);
                }
            },
            {
                Commands.AddHolidays.TrimStart('/'),
                async settings =>
                {
                    await _userSettingsService.UpdateAddHolidaysSettingsAsync(settings);
                    await _messageService.UpdateSettingsListMessageAsync(settings);
                }
            }
        };
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    {
                        if (_messageActions.TryGetValue(update.CallbackQuery.Data.ToLower(), out var action))
                        {
                            if (_getEventsKeys.Contains(update.CallbackQuery.Data.ToLower()))
                            {
                                await action.Invoke(new GetEventsParams()
                                {
                                    ChatId = update.CallbackQuery.Message.Chat.Id,
                                    UserId = update.CallbackQuery.From.Id,
                                    DatePeriod = DatePeriods.ThisWeek
                                });
                            }
                            else
                            if (_settingsKeys.Contains(update.CallbackQuery.Data.ToLower()))
                            {
                                await action.Invoke(new UserSettingsParams()
                                {
                                    ChatId = update.CallbackQuery.Message.Chat.Id,
                                    UserId = update.CallbackQuery.From.Id,
                                });
                            }
                            else
                            {
                                await action.Invoke(new MessageParams()
                                {
                                    ChatId = update.CallbackQuery.Message.Chat.Id,
                                });
                            }
                        }
                        else
                        {
                            if (update.CallbackQuery is { } callbackQuery)
                            {
                                if (_settingsKeys.Any(key => callbackQuery.Data.ToLower().StartsWith(key.TrimStart('/'))))
                                {
                                    var keys = callbackQuery.Data.ToLower().Split('_');
                                    var entity = keys[0];
                                    var entityId = keys[1];
                                    var oldExclude = keys[2] == "1";
                                    if (_updateSettingsActions.TryGetValue(entity, out var updateAction))
                                    {
                                        await updateAction.Invoke(new UpdateUserSettingsParams()
                                        {
                                            ChatId = callbackQuery.Message.Chat.Id,
                                            UserId = update.CallbackQuery.From.Id,
                                            MessageId = callbackQuery.Message.MessageId,
                                            Key = callbackQuery.Data,
                                            EntityId = entityId,
                                            Exclude = !oldExclude,
                                            Markup = callbackQuery.Message.ReplyMarkup
                                        });
                                    }
                                    else
                                    {
                                        throw new NotFoundCommandException();
                                    }
                                }
                                else
                                {
                                    throw new NotFoundCommandException();
                                }
                            }
                            else
                            {
                                throw new NotFoundCommandException();
                            }
                        }
                        return;
                    }
                case UpdateType.Message:
                    {
                        if (_messageActions.TryGetValue(update.Message.Text.ToLower(), out var action))
                        {
                            await action.Invoke(new MessageParams()
                            {
                                ChatId = update.Message.Chat.Id
                            });
                        }
                        else
                        {
                            throw new NotFoundCommandException();
                        }
                        return;
                    }
                default: return;
            }
        }
        catch (NotFoundCommandException)
        {
            await _messageService.NotFoundCommandMessageAsync(new MessageParams()
            {
                ChatId = update.CallbackQuery?.Message?.Chat.Id ?? update?.Message?.Chat.Id ?? throw new Exception("ChatId not found")
            });
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
