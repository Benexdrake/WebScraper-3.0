using Webscraper_API.Scraper.Twitch.Models;

namespace Webscraper_API.Interfaces
{
    public interface ITwitch_API
    {
        Task<User> GetTwitchProfil(string url);
    }
}