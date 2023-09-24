
namespace TelegramBot.Params;

public class EventsMessagesParams : MessageParams
{
    public IList<Models.Events.Event> Events { get; set; }
}
