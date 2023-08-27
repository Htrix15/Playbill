using Microsoft.EntityFrameworkCore;
using Models.Places;

public class ApplicationDbContext : DbContext
{
    public DbSet<Place> Places => Set<Place>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=PlaybillApp.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Place>().HasData(DefaultPlaces.Places);
    }
} 
