namespace Webscraper.API.Interfaces
{
    public interface IIMDb_API
    {
        Task<Movie> GetMovieByUrlAsync(string url);
        Task<List<string>> GetMovieTop250Urls();
        Task<string[]> GetFavoritUrlsAsync(string id);
    }
}