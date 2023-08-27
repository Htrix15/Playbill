namespace Models.Places;

public interface IPlaceRepository
{
    Task<List<Place>> GetPlacesAsync();
    Task AddPlaceAsync(IEnumerable<string> placesNames);
}
