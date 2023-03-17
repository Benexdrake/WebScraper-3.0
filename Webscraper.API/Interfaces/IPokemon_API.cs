namespace Webscraper.API.Interfaces
{
    public interface IPokemon_API
    {
        Task<List<Pokemon>> GetPokemonByIDAsync(int nr);
    }
}