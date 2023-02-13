using Webscraper_API.Scraper.Insight_Digital_Handy.Models;

namespace Webscraper_API.Interfaces
{
    public interface IID_API
    {
        Task<Handy> GetHandyAsync(string url);
        Task<string[]> GetHandyUrls();
    }
}