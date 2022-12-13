namespace EF_Core_Console.Controller;
public class PokemonController : IPokemonController
{
    private readonly Browser _browser;
    private readonly PokemonDBContext _context;
    private readonly IPokemon_API _api;
    public PokemonController(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
        _context = service.GetRequiredService<PokemonDBContext>();
        _api = service.GetRequiredService<IPokemon_API>();
    }

    public async Task FullUpdatePokemons()
    {
        for (int i = 1; i < 1000; i++)
        {
            var pokemon = GetPokemon(i).Result;
            if (pokemon == 0)
                i--;
        }
    }

    public async Task<int> GetPokemon(int nr)
    {
        var p = _context.Pokemons.Where(x => x.Nr == nr).FirstOrDefault();
        Log.Logger.Information($"Pokemon Nr: {nr}");
        if (p is null)
        {
            string url = $"https://www.pokemon.com/de/pokedex/{nr}";
            var doc = _browser.GetPageDocument(url, 1000).Result;
            var pokemon = _api.GetPokemonByIDAsync(nr.ToString(), doc).Result;
            if (pokemon is null)
                return 0;
            else
            {
                SavePokemon(pokemon);
                return 1;
            }
        }
        return 2;
    }

    private void SavePokemon(List<Pokemon> pokemon)
    {
        try
        {
            _context.Pokemons.AddRange(pokemon);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
        }
    }


}
