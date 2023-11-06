using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using TelegramBot.Services;
using TelegramBot.Params;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using TelegramBot.Handlers.Actions.Common;

namespace TelegramBot.Handles;

public class UpdateHandler : IUpdateHandler
{
    private readonly MessageService _messageService;
    private readonly MessageActionsService _messageActionsService;

    private readonly Dictionary<string, IActionMessage> _messageActions;
    private readonly List<string> _getEventsKeys = new();
    private readonly List<string> _settingsKeys = new();

    public UpdateHandler(MessageService messageService, 
        MessageActionsService messageActionsService)
    {
        _messageService = messageService;
        _messageActionsService = messageActionsService;
        _messageActions = _messageActionsService.Get();
        _settingsKeys = _messageActionsService.GetSettingsKeys();
        _getEventsKeys = _messageActionsService.GetEventsKeys();
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
                                await action.CreateMessages(new GetEventsParams()
                                {
                                    ChatId = update.CallbackQuery.Message.Chat.Id,
                                    UserId = update.CallbackQuery.From.Id,
                                    DatePeriod = DatePeriods.ThisWeek
                                });
                            }
                            else
                            if (_settingsKeys.Contains(update.CallbackQuery.Data.ToLower()))
                            {
                                await action.CreateMessages(new UserSettingsParams()
                                {
                                    ChatId = update.CallbackQuery.Message.Chat.Id,
                                    UserId = update.CallbackQuery.From.Id,
                                });
                            }
                            else
                            {
                                await action.CreateMessages(new MessageParams()
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
                                    if (_messageActions.TryGetValue(entity, out var updateAction))
                                    {
                                        await updateAction.CreateMessages(new UpdateUserSettingsParams()
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
                            await action.CreateMessages(new MessageParams()
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
