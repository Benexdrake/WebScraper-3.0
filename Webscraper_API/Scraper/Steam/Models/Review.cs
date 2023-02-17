using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;

public class Review
{
    public bool isPositiv { get; set; }
    public int ReviewCount { get; set; }
}