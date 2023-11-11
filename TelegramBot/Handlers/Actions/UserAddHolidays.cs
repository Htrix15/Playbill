using Models.Search;
using Models.Users;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;
using static TelegramBot.Params.UserSettingsParams;

namespace TelegramBot.Handlers.Actions;

public class UserAddHolidays : SettingsMessageBase<UserSettings>
{
    public UserAddHolidays(MessageService messageService,
        SearchOptions searchOptions,
        IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
    {
        CollbackAction = new Collback(messageService, searchOptions, userSettingsRepository);
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/addholidays";

    public override async Task CreateMessages(Update update)
    {
        var userSettingsRepository = _repository as IUserSettingsRepository;
        var userSettingsParams = update.GetUserSettingsParams();
        var settings = new List<Setting>() {
            new Setting()
            {
                Exclude = ! await userSettingsRepository.GetAddHolidaysAsync(userSettingsParams.UserId),
                Id = 1,
                Label = "Искать в праздничные выходные"
            }
        };
        await CreateMessages(userSettingsParams, settings);
    }

    class Collback : CollbackMessage<UserSettings>
    {
        public Collback(MessageService messageService,
            SearchOptions searchOptions,
            IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
        {
        }
        public override string Command => CollbackCommandHelper.Create(UserAddHolidays.GetCommand());

        public override async Task CreateMessages(Update update)
        {
            var userSettingsRepository = _repository as IUserSettingsRepository;
            var updateUserSettingsParams = update.GetUpdateUserSettingsParams();
            await userSettingsRepository.UpdateAddHolidaysAsync(updateUserSettingsParams.UserId, 
                !updateUserSettingsParams.Exclude);
            await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId, 
                updateUserSettingsParams.MessageId,
                MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));
        }

    }
}
