using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;
public class Video
{
    public int Id { get; set; }
    public string VideoUrl { get; set; }
    public string ThumbnailUrl { get; set; }
}