using Webscraper_API.Scraper.Steam.Models;

namespace Webscraper_API.Interfaces
{
    public interface ISteam_Api
    {
        Task<string[]> GetGameUrls(int category);
        Task<Game> GetSteamGame(string url);
    }
}