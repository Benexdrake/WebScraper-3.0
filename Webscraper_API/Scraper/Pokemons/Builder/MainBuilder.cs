using Webscraper_API.Scraper.Pokemons.Models;

namespace Webscraper_API.Scraper.Pokemons.Builder
{
    public class MainBuilder
    {
        protected Pokemon pokemon = new();
        public PokemonBuilder p => new(pokemon);
    }
}
