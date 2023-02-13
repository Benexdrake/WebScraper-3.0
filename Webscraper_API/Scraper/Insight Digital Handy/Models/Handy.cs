namespace Webscraper_API.Scraper.Insight_Digital_Handy.Models;

public class Handy
{
    public string Id { get; set; }
    public string Manufacturer { get; set; }
    public string Accessible { get; set; }
    public string LaunchGermany { get; set; }
    public string OS { get; set; }
    public string UI { get; set; }
    public string Variants { get; set; }
    public string Prices { get; set; }
    public string Model { get; set; }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public Display Display { get; set; }
    public Casing Casing { get; set; }
    public Hardware Hardware { get; set; }
    public Connectivity Connectivity { get; set; }
    public ConnectionTransmission ConnectionTransmission { get; set; }
    public Camera[] Cameras { get; set; }
    public Miscellaneous Miscellaneous { get; set; }
}
