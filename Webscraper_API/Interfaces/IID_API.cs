namespace Webscraper.API.Interfaces;

public interface IID_API
{
    string Message { get; set; }
    Task<Handy> GetHandyAsync(string url);
    Task<string[]> GetHandyUrls();
}