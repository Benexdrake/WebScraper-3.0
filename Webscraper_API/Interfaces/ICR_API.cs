using Webscraper_API.Scraper.Crunchyroll.Models;

namespace Webscraper_API.Interfaces;

public interface ICR_API
{
    Task<Anime> GetAnimeByUrlAsync(string url, HtmlDocument doc);
    Task<string[]> GetAnimeUrlList(WebDriver driver);
    Task GetEpisode(HtmlDocument doc);
}