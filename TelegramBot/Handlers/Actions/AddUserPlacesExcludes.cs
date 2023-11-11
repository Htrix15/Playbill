using Models.Search;
using Models.Users;
using Models;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;
using Telegram.Bot.Types;
using TelegramBot.Extensions;

namespace TelegramBot.Handlers.Actions;

public class AddUserPlacesExcludes : CollbackMessage<UserSettings>
{
    public AddUserPlacesExcludes(MessageService messageService, SearchOptions searchOptions, IRepository<UserSettings> repository) : base(messageService, searchOptions, repository)
    {
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => CollbackCommandHelper.Create("/addexcludeplaces");

    public override async Task CreateMessages(Update update)
    {
        var userSettingsRepository = _repository as IUserSettingsRepository;
        var updateUserSettingsParams = update.GetUpdateUserSettingsParams();

        var userId = updateUserSettingsParams.UserId;
        var excludes = await userSettingsRepository.GetExcludePlacesIdsAsync(userId) ?? new List<int>();

        var key = int.Parse(updateUserSettingsParams.EntityId);

        if (updateUserSettingsParams.Exclude)
        {   
            excludes.Add(key);
        }
        else
        {
            excludes.Remove(key);
        }
        await userSettingsRepository.UpdateExcludePlacesIdsAsync(userId, excludes);
        await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId,
            updateUserSettingsParams.MessageId,
            MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));

    }
}
