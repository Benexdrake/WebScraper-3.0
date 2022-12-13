using Webscraper_API.Scraper.IMDB.Models;

namespace Webscraper_API.Interfaces
{
    public interface IIMDb_API
    {
        Task<Movie> GetMovieByUrlAsync(string url, HtmlDocument doc);
        Task<List<string>> GetMovieTop250Urls(HtmlDocument doc);
    }
}