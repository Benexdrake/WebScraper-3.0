using Webscraper_API.Scraper.Crunchyroll.Models;

namespace Webscraper_API.Interfaces;

public interface ICR_API
{
    string Message { get; set; }
    Task<Anime> GetAnimeByUrlAsync(string url, int time);
    Task<Anime_Episodes> GetAnimewithEpisodes(string url, int time);
    Task<string[]> GetSimulcastUpdateUrlsAsync();
    Task<string[]> GetDailyUpdateAsync();
    Task<string[]> GetAllAnimeUrlsAsync();
    //Task<Episode[]> GetEpisodes();
    Task<Episode[]> GetEpisodesAsync();
    Task<Episode> GetEpisodeDetails(Episode episode);
}