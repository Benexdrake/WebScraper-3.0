using Webscraper_API.Scraper.Crunchyroll.Models;

namespace Webscraper_API.Interfaces;

public interface ICR_API
{
    Task<Anime> GetAnimeByUrlAsync(string url, int time);
    Task<Anime_Episodes> GetAnimewithEpisodes(string url, int time);
    Task<string[]> GetSimulcastUpdateUrlsAsync();
    Task<string[]> GetAllAnimeUrlsAsync();
    Task<Episode[]> GetEpisodes();
}