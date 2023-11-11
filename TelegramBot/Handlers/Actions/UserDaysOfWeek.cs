using Models.Search;
using Models.Users;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Services;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Helpers;

namespace TelegramBot.Handlers.Actions;

public class UserDaysOfWeek : SettingsMessageBase
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
        var userSettingsParams = update.GetUserSettingsParams();
        var settings = _searchOptions.DaysOfWeek.Select(ConverFunctions<DayOfWeek>()).ToList();
        var userExcludes = await _userSettingsRepository.GetExcludeDaysOfWeekAsync(userSettingsParams.UserId);
        SetExclude(userExcludes, settings);
        await CreateMessages(userSettingsParams, settings);
    }

    class Collback : SettingsMessageBase
    {
        public Collback(MessageService messageService,
            SearchOptions searchOptions,
            IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
        {
        }

        public override string Command => throw new NotImplementedException();

        public override async Task CreateMessages(Update update)
        {
            var updateUserSettingsParams = update.GetUpdateUserSettingsParams();
            var userId = updateUserSettingsParams.UserId;
            var excludes = await _userSettingsRepository.GetExcludeDaysOfWeekAsync(userId);
            var key = Enum.Parse<DayOfWeek>(updateUserSettingsParams.EntityId);
            if (updateUserSettingsParams.Exclude)
            {
                excludes.Add(key);
            }
            else
            {
                excludes.Remove(key);
            }
            await _userSettingsRepository.UpdateExcludeDaysOfWeekAsync(userId, excludes);
            await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId,
                updateUserSettingsParams.MessageId,
                MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));
        }

    }
}
