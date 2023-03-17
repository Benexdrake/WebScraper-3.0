namespace Webscraper.Models.Steam.Models;

public class SteamGame
{
    public string _id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsDLC { get; set; } = false;
    public bool IsEarlyAccess { get; set; } = false;
    public Review[] Review { get; set; } = new Review[0];
    public string GameImageUrl { get; set; } = string.Empty;
    public string[] Screenshots { get; set; } = new string[0];
    public Video[] GameplayVideos { get; set; } = new Video[0];
    public string DescriptionSnippet { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string DevTeam { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string[] Tags { get; set; } = new string[0];
    public bool IsFreeToPlay { get; set; } = false;
    public Price[] Prices { get; set; } = new Price[0];
    public string[] GameFeatures { get; set; } = new string[0];
    public Language[] Languages { get; set; } = new Language[0];
    public PcSpecs[] PcSpecs { get; set; } = new PcSpecs[0];
}
