using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserDaysOfWeek : SettingsMessageBase
{
    public UserDaysOfWeek(MessageService messageService, UserSettingsService userSettingsService) : base(messageService, userSettingsService)
    {
        CollbackAction = new Collback(messageService, userSettingsService);
    }

    public override string Command => Commands.DaysOfWeek;

    public override async Task CreateMessages(BaseParams @params)
    {
        var userSettingsParams = @params as UserSettingsParams;
        await CreateMessages(userSettingsParams,
            await _userSettingsService.GetUserDaysOfWeekSettingsAsync(userSettingsParams.UserId));
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
            await _userSettingsService.UpdateDaysOfWeekSettingsAsync(updateUserSettingsParams);
            await _messageService.UpdateSettingsListMessageAsync(updateUserSettingsParams);
        }

    }
}
