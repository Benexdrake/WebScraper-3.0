using Webscraper_API.Scraper.TCG_Pokemon.Models;

namespace EF_Core_Console.Data;

internal class PokemonCardDbContext : DbContext
{
    public DbSet<PokemonCard> PokemonCards { get; set; }
    public PokemonCardDbContext(DbContextOptions<PokemonCardDbContext> options) : base(options)
    {

    }
}
