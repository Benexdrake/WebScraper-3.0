using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Surface { get; set; }
    public bool Sound { get; set; }
    public bool Subtitle { get; set; }
}