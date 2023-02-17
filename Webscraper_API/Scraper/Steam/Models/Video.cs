using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;
public class Video
{
    public string VideoUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
}