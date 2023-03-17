namespace Webscraper.API.Interfaces
{
    public interface ITCG_M_API
    {
        Task<CardUrl[]> GetAllCardBySetUrl(string setUrl);
        Task<string[]> GetAllSetUrls();
        Task<Card> GetCard(string id);
    }
}