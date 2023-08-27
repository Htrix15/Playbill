using Microsoft.EntityFrameworkCore;
using Models.Places;

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

    public async Task AddPlaceAsync(IEnumerable<string> placesNames)
    {
        await _places.AddRangeAsync(placesNames.Select(placesName => new Place() { Name = placesName }));
        await _applicationDbContext.SaveChangesAsync();
    }
}