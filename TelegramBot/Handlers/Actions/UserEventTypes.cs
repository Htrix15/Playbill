using Models.Events;
using Models.Search;
using Models.Users;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserEventTypes : SettingsMessageBase<UserSettings>
{
    public UserEventTypes(MessageService messageService,
        SearchOptions searchOptions,
        IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
    {
        CollbackAction = new Collback(messageService, searchOptions, userSettingsRepository);
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/eventtypes";

    public override async Task CreateMessages(Update update)
    {
        var userSettingsParams = update.GetUserSettingsParams();
        var settings = _searchOptions.SearchEventTypes?.Select(ConverFunctions<EventTypes>()).ToList() ?? new List<Params.UserSettingsParams.Setting>();

        var userSettingsRepository = _repository as IUserSettingsRepository;

        var userExcludes = new List<EventTypes>();
        if (await userSettingsRepository.UserExsistAsync(userSettingsParams.UserId))
        {
            userExcludes = await userSettingsRepository.GetExcludeEventTypesAsync(userSettingsParams.UserId) ?? userExcludes;
        } 
        else
        {
            userExcludes = _searchOptions.ExcludeSearchEventTypes?.ToList() ?? new List<EventTypes>();
        }

        MarkExcludeSettingsHelper.SetExclude(userExcludes, settings);

        await CreateMessages(userSettingsParams, settings);
    }
    class Collback : CollbackMessage<UserSettings>
    {
        public Collback(MessageService messageService,
            SearchOptions searchOptions,
            IUserSettingsRepository repository) : base(messageService, searchOptions, repository)
        {
        }

        public override string Command => CollbackCommandHelper.Create(UserEventTypes.GetCommand());

        public override async Task CreateMessages(Update update)
        {
            var userSettingsRepository = _repository as IUserSettingsRepository;
            var updateUserSettingsParams = update.GetUpdateUserSettingsParams();
            var userId = updateUserSettingsParams.UserId;
            var excludes = await userSettingsRepository.GetExcludeEventTypesAsync(userId)
                ?? _searchOptions.ExcludeSearchEventTypes?.ToList() 
                ?? new List<EventTypes>();
            var key = Enum.Parse<EventTypes>(updateUserSettingsParams.EntityId);
            if (updateUserSettingsParams.Exclude)
            {
                excludes.Add(key);
            }
            else
            {
                excludes.Remove(key);
            }
            await userSettingsRepository.UpdateExcludeEventTypesAsync(userId, excludes);
            await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId,
                updateUserSettingsParams.MessageId,
                MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));
        }

    }
}