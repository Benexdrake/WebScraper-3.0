namespace EF_Core_Console.Data;

public class PokemonDBContext : DbContext
{
    public DbSet<Pokemon> Pokemons { get; set; }
    public PokemonDBContext(DbContextOptions<PokemonDBContext> options) : base(options)
    {

    }
}
