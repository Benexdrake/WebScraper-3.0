using Webscraper_API.Scraper.TCG_Magic.Model;
using Webscraper_API.Scraper.TCG_Magic.Model.DbModel;

namespace Webscraper_API.Interfaces
{
    public interface ITCG_M_API
    {
        Task<CardUrl[]> GetAllCardBySetUrl(string setUrl);
        Task<string[]> GetAllSetUrls();
        Task<Card> GetCard(string id);
    }
}