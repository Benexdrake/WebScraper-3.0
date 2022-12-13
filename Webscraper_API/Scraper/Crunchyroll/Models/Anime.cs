namespace Webscraper_API.Scraper.Crunchyroll.Models
{
    public class Anime
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Episodes { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Rating { get; set; }
        public string Tags { get; set; }
        public string Publisher { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
