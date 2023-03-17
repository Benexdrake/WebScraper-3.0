namespace Webscraper.API.Interfaces
{
    public interface IOP_API
    {
        Task GetPhoneByUrl(string url, int time);
        Task<string[]> GetPhoneUrls();
    }
}