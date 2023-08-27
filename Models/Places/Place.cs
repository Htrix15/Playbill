namespace Models.Places;

public class Place
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public Place? Parent { get; set; }
    public List<Place> Synonyms { get; set; }
}
