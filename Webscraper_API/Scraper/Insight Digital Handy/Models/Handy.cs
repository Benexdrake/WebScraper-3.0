namespace Webscraper_API.Scraper.Insight_Digital_Handy.Models;

public class Handy
{
    public string Id { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Accessible { get; set; } = string.Empty;
    public string LaunchGermany { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string UI { get; set; } = string.Empty;
    public string Variants { get; set; } = string.Empty;
    public string Prices { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Display Display { get; set; }
    public Casing Casing { get; set; }
    public Hardware Hardware { get; set; }
    public Connectivity Connectivity { get; set; }
    public ConnectionTransmission ConnectionTransmission { get; set; }
    public Camera[] Cameras { get; set; }
    public Miscellaneous Miscellaneous { get; set; }
}
