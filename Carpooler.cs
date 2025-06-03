using Microsoft.EntityFrameworkCore;
public class Carpooler
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime Date { get; set; }
    public decimal Reward { get; set; }
}

public class CarpoolContext : DbContext
{
    public DbSet<Carpooler> Carpoolers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite("Data Source=carpool.db");
    }
}