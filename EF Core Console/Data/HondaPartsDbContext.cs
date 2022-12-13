using Webscraper_API.Scraper.Honda.Models;

namespace EF_Core_Console.Data;

public class HondaPartsDbContext : DbContext
{
    public DbSet<NewParts> HondaParts { get; set; }
    //public DbSet<Accessories> Accessories { get; set; }
    //public DbSet<PartFits> PartFits { get; set; }
    //public DbSet<PartFitment> PartFitments { get; set; }
    public HondaPartsDbContext(DbContextOptions<HondaPartsDbContext> options) : base(options)
    {

    }
}
