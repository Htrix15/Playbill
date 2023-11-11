using Microsoft.EntityFrameworkCore;
using Models.Places;
using System.Linq;
using System.Linq.Expressions;

namespace Repository;

public class PlaceRespository : IPlaceRepository
{
    private readonly DbSet<Place> _places;
    private readonly ApplicationDbContext _applicationDbContext;
    public PlaceRespository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
        _places = applicationDbContext.Places;
    }

    public async Task<List<Place>> GetPlacesAsync() => await _places
        .AsNoTracking()
        .Include(place => place.Synonyms)
        .ToListAsync();

    public async Task<List<Place>> GetPlacesAsync(IEnumerable<string> placeNames) => await _places
        .AsNoTracking()
        .Where(place => placeNames.Contains(place.Name))
        .ToListAsync();

    public async Task<List<Place>> GetPlacesAsync(IEnumerable<int> placeIds) => await _places
        .AsNoTracking()
        .Where(place => placeIds.Contains(place.Id))
        .ToListAsync();
    public async Task<List<TResult>> GetPlacesAsync<TResult>(IEnumerable<int> placeIds, Expression<Func<Place, TResult>> selector) => await _places
        .AsNoTracking()
        .Where(place => placeIds.Contains(place.Id))
        .Select(selector)
        .ToListAsync();

    public async Task AddPlaceAsync(IEnumerable<string> placesNames)
    {
        await _places.AddRangeAsync(placesNames.Select(placesName => new Place() { Name = placesName }));
        await _applicationDbContext.SaveChangesAsync();
    }
}