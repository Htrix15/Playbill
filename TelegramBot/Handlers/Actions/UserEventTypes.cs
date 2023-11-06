using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserEventTypes : SettingsMessageBase
{
    public UserEventTypes(MessageService messageService, UserSettingsService userSettingsService) : base(messageService, userSettingsService)
    {
        CollbackAction = new Collback(messageService, userSettingsService);
    }

    public override string Command => Commands.EventTypes;

    public override async Task CreateMessages(BaseParams @params)
    {
        var userSettingsParams = @params as UserSettingsParams;
        await CreateMessages(userSettingsParams,
            await _userSettingsService.GetUserEventTypesSettingsAsync(userSettingsParams.UserId));
    }
    class Collback : SettingsMessageBase
    {
        public Collback(MessageService messageService, UserSettingsService userSettingsService) : base(messageService, userSettingsService)
        {
        }

        public override string Command => throw new NotImplementedException();

        public override async Task CreateMessages(BaseParams @params)
        {
            var updateUserSettingsParams = @params as UpdateUserSettingsParams;
            await _userSettingsService.UpdateEventTypesSettingsAsync(updateUserSettingsParams);
            await _messageService.UpdateSettingsListMessageAsync(updateUserSettingsParams);
        }

    }
}