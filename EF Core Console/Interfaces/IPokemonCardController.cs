namespace EF_Core_Console.Interfaces
{
    public interface IPokemonCardController
    {
        Task FullUpdatePokemonCards();
        Task GetPokemonCard(string url);
    }
}