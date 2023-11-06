using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserAddHolidays : SettingsMessageBase
{
    public UserAddHolidays(MessageService messageService, UserSettingsService userSettingsService) : base(messageService, userSettingsService)
    {
        CollbackAction = new Collback(messageService, userSettingsService);
    }

    public override string Command => Commands.AddHolidays;

    public override async Task CreateMessages(BaseParams @params)
    {
        var userSettingsParams = @params as UserSettingsParams;
        await CreateMessages(userSettingsParams,
            await _userSettingsService.GetUserAddHolidaysSettingsAsync(userSettingsParams.UserId));
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
            await _userSettingsService.UpdateAddHolidaysSettingsAsync(updateUserSettingsParams);
            await _messageService.UpdateSettingsListMessageAsync(updateUserSettingsParams);
        }

    }
}
