namespace EF_Core_Console.Interfaces
{
    public interface IPokemonController
    {
        Task FullUpdatePokemons();
        Task<int> GetPokemon(int nr);
    }
}