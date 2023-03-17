using Webscraper.Models.Pokemons.Models;

namespace Webscraper.Models.Pokemons.Builder
{
    public class MainBuilder
    {
        protected Pokemon pokemon = new();
        public PokemonBuilder p => new(pokemon);
    }
}
