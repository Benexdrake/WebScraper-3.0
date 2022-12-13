using Webscraper_API.Scraper.Pokemons.Models;

namespace Webscraper_API.Interfaces
{
    public interface IPokemon_API
    {
        Task<List<Pokemon>> GetPokemonByIDAsync(string id, HtmlDocument doc);
    }
}