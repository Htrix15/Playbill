using Models.Events;
using Models.Search;
using Models.Users;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserEventTypes : SettingsMessageBase
{
    public UserEventTypes(MessageService messageService,
        SearchOptions searchOptions,
        IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
    {
        CollbackAction = new Collback(messageService, searchOptions, userSettingsRepository);
    }

    public override string Command => Commands.EventTypes;

    public override async Task CreateMessages(Update update)
    {
        var userSettingsParams = update.GetUserSettingsParams();
        var settings = _searchOptions.SearchEventTypes.Select(ConverFunctions<EventTypes>()).ToList();
        var userExcludes = await _userSettingsRepository.GetExcludeEventTypesAsync(userSettingsParams.UserId);
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
            var excludes = await _userSettingsRepository.GetExcludeEventTypesAsync(userId);
            var key = Enum.Parse<EventTypes>(updateUserSettingsParams.EntityId);
            if (updateUserSettingsParams.Exclude)
            {
                excludes.Add(key);
            }
            else
            {
                excludes.Remove(key);
            }
            await _userSettingsRepository.UpdateExcludeEventTypesAsync(userId, excludes);
            await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId,
                updateUserSettingsParams.MessageId,
                MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));
        }

    }
}