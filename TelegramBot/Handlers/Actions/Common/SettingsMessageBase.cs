using Models;
using Models.Search;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Extensions;
using TelegramBot.Helpers;
using TelegramBot.Params;
using TelegramBot.Services;
using static TelegramBot.Params.UserSettingsParams;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class SettingsMessageBase<T> : MessageBase
{
    protected readonly SearchOptions _searchOptions;
    protected readonly IRepository<T> _repository;
    protected string CommandKey => CommandKeyHelper.Create(Command);

    public string MessageText => "Нажмите на то, что хотите добавить или удалить";

    protected CollbackMessage<T> CollbackAction;

    public SettingsMessageBase(MessageService messageService, 
        SearchOptions searchOptions,
        IRepository<T> repository) : base(messageService)
    {
        _searchOptions = searchOptions;
        _repository = repository;
    }

    protected Func<T, Setting> ConverFunctions<T>() => item => new Setting()
    {
        Exclude = false,
        Id = Convert.ToInt32(item),
        Label = item?.ToString() ?? string.Empty
    };

    protected async Task CreateMessages(UserSettingsParams @params, List<Setting> settings)
    {
        var buttons = new List<InlineKeyboardButton[]>();
        buttons.AddRange(MarkupHelper.CreateToogleButtons(settings, CommandKey));
        buttons.AddRange(MarkupHelper.SettingsSet);
        await _messageService.SendMessageAsync(@params.ChatId, MessageText, buttons.ToInlineKeyboardMarkup());
    }

    public override void InsertTo(Dictionary<string, IActionMessage> dictionary)
    {
        dictionary.Add(Command, this);
        if (CollbackAction != null)
        {
            dictionary.Add(CollbackAction.Command, CollbackAction);
        }
    }
}

public abstract class SettingsMessageBase<T1, T2> : SettingsMessageBase<T1>
{
    protected readonly IRepository<T2> _supportRepository;
    protected SettingsMessageBase(MessageService messageService, SearchOptions searchOptions, IRepository<T1> repository, IRepository<T2> supportRepository) 
        : base(messageService, searchOptions, repository)
    {
        _supportRepository = supportRepository;
    }
}