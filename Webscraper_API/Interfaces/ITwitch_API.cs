namespace Webscraper.API.Interfaces
{
    public interface ITwitch_API
    {
        Task<TwitchUser> GetTwitchProfil(string url);
    }
}