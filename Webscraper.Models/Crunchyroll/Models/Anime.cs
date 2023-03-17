namespace Webscraper.Models.Crunchyroll.Models
{
    public class Anime
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Tags { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int Episodes { get; set; }
    }
}
