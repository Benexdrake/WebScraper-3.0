using Webscraper_API.Scraper.Steam.Models;

namespace Webscraper_API.Scraper.Steam.Models;

public class Game
{
    public string Id { get; set; }
    public string Title { get; set; }
    public Review[] Review { get; set; }
    public string GameImageUrl { get; set; }
    public string[] Screenshots { get; set; }
    public Video[] GameplayVideos { get; set; }
    public string DescriptionSnippet { get; set; }
    public string Description { get; set; }
    public string ReleaseDate { get; set; }
    public string DevTeam { get; set; }
    public string Publisher { get; set; }
    public string[] Tags { get; set; }
    public bool IsFreeToPlay { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public string[] GameFeatures { get; set; }
    public Language[] Languages { get; set; }
    public PcSpecs[] PcSpecs { get; set; }
}
