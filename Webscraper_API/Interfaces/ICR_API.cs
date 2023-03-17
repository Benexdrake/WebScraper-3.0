namespace Webscraper.API.Interfaces;

public interface ICR_API
{
    string Message { get; set; }
    Task<string[]> GetAllAnimeUrlsAsync();
    Task<string[]> GetWeeklyUpdateAsync();
    Task<string[]> GetDailyUpdateAsync();
    Task<Anime_Episodes> GetAnimewithEpisodes(string url, int time);
    Task<Anime> GetAnimeByUrlAsync(string url, int time);
    Task<Episode[]> GetEpisodesAsync();
    Task<Episode> GetEpisodeDetails(Episode episode);
}