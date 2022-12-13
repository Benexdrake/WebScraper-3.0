using Webscraper_API.Scraper.TCG_Pokemon.Models;

namespace EF_Core_Console.Controller;
public class PokemonCardController : IPokemonCardController
{
    private readonly Browser _browser;
    private readonly ITCG_API _api;
    private readonly PokemonCardDbContext _context;
    public PokemonCardController(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
        _api = service.GetRequiredService<ITCG_API>();
        _context = service.GetRequiredService<PokemonCardDbContext>();
    }

    public async Task FullUpdatePokemonCards()
    {
        string url = $"https://www.pokemon.com/de/pokemon-sammelkartenspiel/pokemon-karten/1?cardName=&cardText=&evolvesFrom=&card-grass=on&card-fire=on&card-water=on&card-lightning=on&card-psychic=on&card-fighting=on&card-darkness=on&card-metal=on&card-colorless=on&card-fairy=on&card-dragon=on&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist=&sort=number&sort=number";
        var doc = _browser.GetPageDocument(url, 0).Result;
        var max = _api.GetMaxPokemonCards(doc).Result;
        List<string> urlList = new List<string>();
        for (int i = 1; i <= max; i++)
        {
            Log.Information($"{Helper.Percent(i, max)} / 100");
            url = $"https://www.pokemon.com/de/pokemon-sammelkartenspiel/pokemon-karten/{i}?cardName=&cardText=&evolvesFrom=&card-grass=on&card-fire=on&card-water=on&card-lightning=on&card-psychic=on&card-fighting=on&card-darkness=on&card-metal=on&card-colorless=on&card-fairy=on&card-dragon=on&simpleSubmit=&format=unlimited&hitPointsMin=0&hitPointsMax=340&retreatCostMin=0&retreatCostMax=5&totalAttackCostMin=0&totalAttackCostMax=5&particularArtist=&sort=number&sort=number";
            var doc2 = _browser.GetPageDocument(url, 0).Result;
            var urls = _api.GetUrlsFromSite(doc2).Result;
            urlList.AddRange(urls);
        }

        foreach (var u in urlList)
        {
            await GetPokemonCard(u);
        }
    }

    public async Task GetPokemonCard(string url)
    {
        var card = _context.PokemonCards.Where(x => x.Url.Equals(url)).FirstOrDefault();
        if (card is null)
        {
            var doc3 = _browser.GetPageDocument(url, 0).Result;
            var pokeCard = _api.GetPokemonCardAsync(url, doc3).Result;
            SavePokemonCard(pokeCard);
        }
    }

    private void SavePokemonCard(PokemonCard card)
    {
        try
        {
            _context.PokemonCards.Add(card);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
        }
    }
}
