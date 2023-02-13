using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;

public class Review
{
    public int Id { get; set; }
    public bool isPositiv { get; set; }
    public int ReviewCount { get; set; }
}