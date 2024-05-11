namespace Models.Events;

public class EventsResult
{
    public List<Event> Result { get; set; } = [];
    public List<Event> SubstandardEvents { get; set; } = [];
}
