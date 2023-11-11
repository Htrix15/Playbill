using Models.Billboards;
using Models.Search;
using Models.Users;
using Telegram.Bot.Types;
using TelegramBot.Extensions;
using TelegramBot.Handlers.Actions.Common;
using TelegramBot.Helpers;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions;

public class UserBillboards : SettingsMessageBase
{
    public UserBillboards(MessageService messageService,
        SearchOptions searchOptions,
        IUserSettingsRepository userSettingsRepository) : base(messageService, searchOptions, userSettingsRepository)
    {
        CollbackAction = new Collback(messageService, searchOptions, userSettingsRepository);
    }
    public override string Command => GetCommand();
    public static new string GetCommand() => "/billboards";

    public override async Task CreateMessages(Update update)
    {
        var userSettingsParams = update.GetUserSettingsParams();
        var settings = _searchOptions.SupportedBillboards?.Select(ConverFunctions<BillboardTypes>()).ToList() ?? new List<Params.UserSettingsParams.Setting>();

        var userExcludes = new List<BillboardTypes>();
        if (await _userSettingsRepository.UserExsistAsync(userSettingsParams.UserId))
        {
            userExcludes = await _userSettingsRepository.GetExcludeBillboardsAsync(userSettingsParams.UserId) ?? userExcludes;
        } 
        else
        {
            userExcludes = _searchOptions.ExcludeBillboards?.ToList() ?? new List<BillboardTypes>();
        }

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
            var excludes = await _userSettingsRepository.GetExcludeBillboardsAsync(userId) 
                ?? _searchOptions.ExcludeBillboards?.ToList()
                ?? new List<BillboardTypes>();
            var key = Enum.Parse<BillboardTypes>(updateUserSettingsParams.EntityId);
            if (updateUserSettingsParams.Exclude)
            {
                excludes.Add(key);
            }
            else
            {
                excludes.Remove(key);
            }
            await _userSettingsRepository.UpdateExcludeBillboardsAsync(userId, excludes);
            await _messageService.EditMessageAsync(updateUserSettingsParams.ChatId,
                updateUserSettingsParams.MessageId,
                MarkupHelper.ReplaceButtons(updateUserSettingsParams.Markup, updateUserSettingsParams.Key));
        }
    }
}
