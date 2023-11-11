using Models;
using Models.Places;
using Models.Search;
using Models.Users;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Params;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserPlaces : SettingsMessageBase<UserSettings, Place>
{
    public UserPlaces(MessageService messageService, SearchOptions searchOptions, IUserSettingsRepository repository, IPlaceRepository supportRepository) : base(messageService, searchOptions, repository, supportRepository)
    {
        CollbackAction = new Collback(messageService, searchOptions, repository);
    }

    public override string Command => GetCommand();
    public static new string GetCommand() => "/places";

    private string NotFoundExcludesMessage = "У вас нет исключений. Их можно будет добавить после поиска";

    public override async Task CreateMessages(Update update)
    {
        var userSettingsRepository = _repository as IUserSettingsRepository;
        var userSettingsParams = update.GetUserSettingsParams();
        var userExcludes = new List<int>();
        if (await userSettingsRepository.UserExsistAsync(userSettingsParams.UserId))
        {
            userExcludes = await userSettingsRepository.GetExcludePlacesIdsAsync(userSettingsParams.UserId) ?? userExcludes;
        }
        else
        {
            await _messageService.SendMessageAsync(userSettingsParams.ChatId, NotFoundExcludesMessage, MarkupHelper.SettingsSet.ToInlineKeyboardMarkup());
            return;
        }

        if(!userExcludes.Any())
        {
            await _messageService.SendMessageAsync(userSettingsParams.ChatId, NotFoundExcludesMessage, MarkupHelper.SettingsSet.ToInlineKeyboardMarkup());
            return;
        }

        var placeRepository = _supportRepository as IPlaceRepository;
        var places = await placeRepository.GetPlacesAsync(userExcludes);

        if (!places.Any() && userExcludes.Any())
        {
            await userSettingsRepository.UpdateExcludePlacesIdsAsync(userSettingsParams.UserId, new List<int>());
            await _messageService.SendMessageAsync(userSettingsParams.ChatId, NotFoundExcludesMessage, MarkupHelper.SettingsSet.ToInlineKeyboardMarkup());
            return;
        }
        var settings = places.Select(place => new UserSettingsParams.Setting()
        {
            Id = place.Id,
            Label = place.Name
        }).ToList();

        await CreateMessages(userSettingsParams, settings);
    }

    class Collback : CollbackMessage<UserSettings>
    {
        public Collback(MessageService messageService, SearchOptions searchOptions, IRepository<UserSettings> repository) : base(messageService, searchOptions, repository)
        {
        }
        public override string Command => CollbackCommandHelper.Create(UserPlaces.GetCommand());

        public override async Task CreateMessages(Update update)
        {
            var userSettingsRepository = _repository as IUserSettingsRepository;
            var updateUserSettingsParams = update.GetUpdateUserSettingsParams();

            var userId = updateUserSettingsParams.UserId;
            var excludes = await userSettingsRepository.GetExcludePlacesIdsAsync(userId) ?? new List<int>();

            var key = int.Parse(updateUserSettingsParams.EntityId);

            if (updateUserSettingsParams.Exclude)
            {
                excludes.Remove(key);
            }
            else
            {
                excludes.Add(key);
            }
            await userSettingsRepository.UpdateExcludePlacesIdsAsync(userId, excludes);
            await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId,
                updateUserSettingsParams.MessageId,
                MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));

        }
    }
}
