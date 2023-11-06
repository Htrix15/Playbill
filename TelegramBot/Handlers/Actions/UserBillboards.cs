using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserBillboards : SettingsMessageBase
{
    public UserBillboards(MessageService messageService, UserSettingsService userSettingsService) : base(messageService, userSettingsService)
    {
        CollbackAction = new Collback(messageService, userSettingsService);
    }

    public override string Command => Commands.Billboards;

    public override async Task CreateMessages(BaseParams @params)
    {
        var userSettingsParams = @params as UserSettingsParams;
        await CreateMessages(userSettingsParams,
            await _userSettingsService.GetUserBillboardsSettingsAsync(userSettingsParams.UserId));   
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
            await _userSettingsService.UpdateBillboardsSettingsAsync(updateUserSettingsParams);
            await _messageService.UpdateSettingsListMessageAsync(updateUserSettingsParams);
        }
    }
}
