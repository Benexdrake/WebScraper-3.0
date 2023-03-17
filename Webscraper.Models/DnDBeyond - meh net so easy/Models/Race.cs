namespace Webscraper_API.Scraper.DnDBeyond.Models;

public class Race
{
    public int Id { get; set; }
    public string RaceName { get; set; }
    public string[] MaleNames { get; set; }
    public string[] FemaleNames { get; set; }
    public string[] ClanNames { get; set; }
    public string AbilityScore { get; set; }
    public string Age { get; set; }
    public string Size { get; set; }
    public string Speed { get; set; }
    public string Darkvision { get; set; }
    public string Resilience { get; set; }
}
