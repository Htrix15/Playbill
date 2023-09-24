using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models.Billboards;
using Models.Events;
using Models.Places;
using Models.Users;
using System.Text.Json;

public class ApplicationDbContext : DbContext
{
    public DbSet<Place> Places => Set<Place>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=PlaybillApp.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Place>().HasData(DefaultPlaces.Places);

        modelBuilder.Entity<UserSettings>()
            .Property(us => us.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<UserSettings>()
            .Property(us => us.ExcludeBillboards)
            .HasConversion(
                excludeBillboards => JsonSerializer.Serialize(excludeBillboards, (JsonSerializerOptions)null), 
                json => JsonSerializer.Deserialize<List<BillboardTypes>>(json, (JsonSerializerOptions)null),
                new ValueComparer<List<BillboardTypes>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        modelBuilder.Entity<UserSettings>()
            .Property(us => us.ExcludeEventTypes)
            .HasConversion(
                excludeEventTypes => JsonSerializer.Serialize(excludeEventTypes, (JsonSerializerOptions)null),
                json => JsonSerializer.Deserialize<List<EventTypes>>(json, (JsonSerializerOptions)null),
                new ValueComparer<List<EventTypes>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
  
        modelBuilder.Entity<UserSettings>()
            .Property(us => us.ExcludeDaysOfWeek)
            .HasConversion(
                excludeDaysOfWeek => JsonSerializer.Serialize(excludeDaysOfWeek,(JsonSerializerOptions)null),
                json => JsonSerializer.Deserialize<List<DayOfWeek>>(json, (JsonSerializerOptions)null),
                new ValueComparer<List<DayOfWeek>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        modelBuilder.Entity<UserSettings>()
            .Property(us => us.ExcludePlacesIds)
            .HasConversion(
                excludePlaces => JsonSerializer.Serialize(excludePlaces,(JsonSerializerOptions)null),
                json => JsonSerializer.Deserialize<List<int>>(json, (JsonSerializerOptions)null),
                new ValueComparer<List<int>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
    }
} 
