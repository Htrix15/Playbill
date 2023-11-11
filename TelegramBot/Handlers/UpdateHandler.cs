using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using TelegramBot.Services;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Extensions;
using TelegramBot.Exceptions;
using TelegramBot.Helpers;

namespace TelegramBot.Handles;

public class UpdateHandler : IUpdateHandler
{
    private readonly MessageService _messageService;

    private readonly Dictionary<string, IActionMessage> _messageActions;

    public UpdateHandler(MessageService messageService, 
        MessageActionsService messageActionsService)
    {
        _messageService = messageService;
        _messageActions = messageActionsService.Get();
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (_messageActions.TryGetValue(update.GetKey(), out var action))
            {
                await action.CreateMessages(update);
            }
            else
            {
                throw new NotFoundCommandException();
            }
        }
        catch (NotFoundCommandException)
        {
            var chatId = update.CallbackQuery?.Message?.Chat.Id ?? update?.Message?.Chat.Id ?? throw new Exception("ChatId not found");
            await _messageService.SendMessageAsync(chatId, 
                "Бот не знает такой команды :(", 
                MarkupHelper.GetStartButtons());
        }
        catch (ApiRequestException exception) when (exception.ErrorCode == 429)
        {
            await Task.Delay(1000 * exception.Parameters?.RetryAfter ?? 1, cancellationToken);
            throw exception;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            var chatId = update.CallbackQuery?.Message?.Chat.Id ?? update?.Message?.Chat.Id ?? throw new Exception("ChatId not found");
            await _messageService.SendMessageAsync(chatId,
               "Что-то пошло не так :(", 
               MarkupHelper.GetStartButtons());
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
