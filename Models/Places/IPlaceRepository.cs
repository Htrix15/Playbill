using System.Linq.Expressions;

namespace Models.Places;

public interface IPlaceRepository: IRepository<Place>
{
    Task<List<Place>> GetPlacesAsync();
    Task<List<Place>> GetPlacesAsync(IEnumerable<string> placeNames);
    Task<List<Place>> GetPlacesAsync(IEnumerable<int> placeIds);
    Task<List<TResult>> GetPlacesAsync<TResult>(IEnumerable<int> placeIds, Expression<Func<Place, TResult>> selector);
    Task AddPlaceAsync(IEnumerable<string> placesNames);
}
