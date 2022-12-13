using Webscraper_API.Scraper.Honda.Models;

namespace Webscraper_API.Interfaces
{
    public interface IHonda_Api
    {
        Task<Accessories> GetAccessories(string url, HtmlDocument doc);
        Task<string[]> GetAccessoriesUrls(HtmlDocument doc);
        Task<CategoryUrl[]> GetCategoriesUrls(HtmlDocument doc);
        Task<string[]> GetMainCategoryUrls(HtmlDocument doc);
        Task<NewParts> GetPart(HtmlDocument doc, CategoryUrl category);
        Task<string[]> GetPartsUrls(HtmlDocument doc);
        Task<string[]> GetRelatedPartsUrls(HtmlDocument doc);
    }
}