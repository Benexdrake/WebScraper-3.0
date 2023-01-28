namespace EF_Core_Console;
public class Main : IHostedService
{
    private readonly ICrunchyrollController _crunchyrollController;
    private readonly IIMDbController _iMDbController;
    private readonly IPokemonController _pokemonController;
    private readonly IPokemonCardController _pokemonCardController;
    private readonly IHondaPartsController _hondaPartsController;
    private readonly MagicController _magicController;
    private readonly IphoneController _iphoneController;
    private readonly OnePlusController _onePlusController;

    private readonly Browser _browser;

    public Main(IServiceProvider service)
    {
        _crunchyrollController = service.GetRequiredService<ICrunchyrollController>();
        _iMDbController = service.GetRequiredService<IIMDbController>();
        _pokemonController = service.GetRequiredService<IPokemonController>();
        _pokemonCardController= service.GetRequiredService<IPokemonCardController>();
        _hondaPartsController = service.GetRequiredService<IHondaPartsController>();
        _magicController = service.GetRequiredService<MagicController>();
        _iphoneController= service.GetRequiredService<IphoneController>();
        _onePlusController= service.GetRequiredService<OnePlusController>();
        _browser= service.GetRequiredService<Browser>();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _browser.WebDriver = _browser.FirefoxDebug();

        // Crunchyroll Debug with Url

        //await _crunchyrollController.Debug("https://www.crunchyroll.com/de/series/GVDHX8QNW/chainsaw-man");
        //await _crunchyrollController.SimulcastUpdate();

        // Crunchyroll Full Update
        //await _crunchyrollController.FullUpdateAnimes();

        // Imdb Single Movie with Url
        //await _iMDbController.GetMovie("https://www.imdb.com/title/tt0111161/");
        //var movie = await _iMDbController.GetMovie("https://www.imdb.com/title/tt1630029/");

        // Imdb Top 250
        //await _iMDbController.LoadTop250();
        //await _iMDbController.GetFavorits("ls000471608");

        // Pokemon Single Pokemon with Nr
        //await _pokemonController.GetPokemon(3);
        // All Pokemons
        //await _pokemonController.FullUpdatePokemons();

        // All Pokemon Cards
        //await _pokemonCardController.FullUpdatePokemonCards();

        // All Honda Parts
        //await _hondaPartsController.FullUpdate(true, "https://www.hondapartsnow.com/honda-s2000-parts.html");
        //await _hondaPartsController.GetPart(new CategoryUrl(){ Category = "-", SubCategory = "-", Url = "https://www.hondapartsnow.com/genuine/honda~bearing~assy~91002-pcz-003.html" });

        // Magic TCG
        //await _magicController.Test();

        // Iphones

        //await _iphoneController.GetAllIphones();

        // One Plus Phones

        //await _onePlusController.AllPhones();

        Console.WriteLine();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
