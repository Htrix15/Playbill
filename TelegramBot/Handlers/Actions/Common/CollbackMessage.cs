using Models;
using Models.Search;
using TelegramBot.Services;

namespace TelegramBot.Handlers.Actions.Common;

public abstract class CollbackMessage<T> : MessageBase
{
    protected readonly SearchOptions _searchOptions;
    protected readonly IRepository<T> _repository;
    public CollbackMessage(MessageService messageService, 
        SearchOptions searchOptions,
        IRepository<T> repository) : base(messageService)
    {
        _searchOptions = searchOptions;
        _repository = repository;
    }

    public override string Command => throw new NotImplementedException();

}