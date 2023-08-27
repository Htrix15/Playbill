using Models.Events;

namespace Models.ProcessingServices.EventsGrouping;

public class PlaceSynonyms
{
    public string? Place { get; set; }
    public HashSet<string>? Synonyms { get; set; }
    public void SetPlaceName(Event @event)
    {
        if (@event.Place.Equals(Place)) return;
        if (Synonyms.Contains(@event.Place))
        {
            @event.Place = Place;
        }
    }
}
