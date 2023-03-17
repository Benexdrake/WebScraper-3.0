namespace Webscraper.API.Interfaces
{
    public interface ISteam_Api
    {
        string Message { get; set; }
        Task<App[]> GetAllGameIds();
        Task<string[]> GetGameUrls(int category);
        Task<SteamGame> GetSteamGame(string url);

        Task<string[]> GetGameUrlsFromWishlist(string url);
        Task<SteamUser> GetUser(string url);
    }
}