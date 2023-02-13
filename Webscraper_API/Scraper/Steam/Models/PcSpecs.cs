using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;

public class PcSpecs
{
    public int Id { get; set; }
    public string Information { get; set; }
    public string OS { get; set; }
    public string CPU { get; set; }
    public string RAM { get; set; }
    public string GPU { get; set; }
    public string DirectX { get; set; }
    public string HardDisk { get; set; }
    public string AdditionalNote { get; set; }
}
