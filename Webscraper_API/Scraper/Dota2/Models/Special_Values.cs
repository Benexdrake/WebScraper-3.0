namespace Webscraper_API.Scraper.Dota2.Models;
public class Special_Values
{
    public string name { get; set; }
    public float[] values_float { get; set; }
    public bool is_percentage { get; set; }
    public string heading_loc { get; set; }
    public object[] bonuses { get; set; }
}
