using Microsoft.EntityFrameworkCore;

namespace Webscraper_API.Scraper.Steam.Models;

public class PcSpecs
{
    public string Information { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string CPU { get; set; } = string.Empty;
    public string RAM { get; set; } = string.Empty;
    public string GPU { get; set; } = string.Empty;
    public string DirectX { get; set; } = string.Empty;
    public string HardDisk { get; set; } = string.Empty;
    public string AdditionalNote { get; set; } = string.Empty;
}
