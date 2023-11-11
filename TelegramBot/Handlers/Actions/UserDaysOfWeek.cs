using Models.Search;
using Models.Users;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Helpers;

namespace TelegramBot.Handlers.Actions;

public class UserDaysOfWeek : SettingsMessageBase<UserSettings>
{
    public UserDaysOfWeek(MessageService messageService,
        SearchOptions searchOptions,
        IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
    {
        CollbackAction = new Collback(messageService, searchOptions, userSettingsRepository);
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/daysofweek";

    public override async Task CreateMessages(Update update)
    {
        var userSettingsRepository = _repository as IUserSettingsRepository;
        var userSettingsParams = update.GetUserSettingsParams();
        var settings = _searchOptions.DaysOfWeek.Select(ConverFunctions<DayOfWeek>()).ToList();
        var userExcludes = await userSettingsRepository.GetExcludeDaysOfWeekAsync(userSettingsParams.UserId);
        MarkExcludeSettingsHelper.SetExclude(userExcludes, settings);
        await CreateMessages(userSettingsParams, settings);
    }

    class Collback : CollbackMessage<UserSettings>
    {
        public Collback(MessageService messageService,
            SearchOptions searchOptions,
            IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
        {
        }

        public override string Command => CollbackCommandHelper.Create(UserDaysOfWeek.GetCommand());

        public override async Task CreateMessages(Update update)
        {
            var userSettingsRepository = _repository as IUserSettingsRepository;
            var updateUserSettingsParams = update.GetUpdateUserSettingsParams();
            var userId = updateUserSettingsParams.UserId;
            var excludes = await userSettingsRepository.GetExcludeDaysOfWeekAsync(userId);
            var key = Enum.Parse<DayOfWeek>(updateUserSettingsParams.EntityId);
            if (updateUserSettingsParams.Exclude)
            {
                excludes.Add(key);
            }
            else
            {
                excludes.Remove(key);
            }
            await userSettingsRepository.UpdateExcludeDaysOfWeekAsync(userId, excludes);
            await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId,
                updateUserSettingsParams.MessageId,
                MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));
        }

    }
}
