namespace Webscraper.API.Interfaces
{
    public interface IDota_Api
    {
        Task<int[]> GetAllIds();
        Task<Hero> GetHero(int id);
    }
}