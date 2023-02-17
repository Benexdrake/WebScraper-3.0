using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;

public class Language
{
    public string Name { get; set; } = string.Empty;
    public bool Surface { get; set; }
    public bool Sound { get; set; }
    public bool Subtitle { get; set; }
}