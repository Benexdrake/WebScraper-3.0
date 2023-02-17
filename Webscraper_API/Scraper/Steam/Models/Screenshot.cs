using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;

public class Screenshot
{
    public int Id { get; set; }
    public string S { get; set; } = string.Empty;
}
