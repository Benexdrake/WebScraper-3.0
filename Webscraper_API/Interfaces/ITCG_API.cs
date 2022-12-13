using Webscraper_API.Scraper.TCG_Pokemon.Models;

namespace Webscraper_API.Interfaces
{
    public interface ITCG_API
    {
        Task<int> GetMaxPokemonCards(HtmlDocument doc);
        Task<PokemonCard> GetPokemonCardAsync(string url, HtmlDocument doc);
        Task<List<string>> GetUrlsFromSite(HtmlDocument doc);
    }
}