using Webscraper_API.Scraper.Dota2.Models;

namespace Webscraper_API.Interfaces
{
    public interface IDota_Api
    {
        Task<int[]> GetAllIds();
        Task<Hero> GetHero(int id);
    }
}