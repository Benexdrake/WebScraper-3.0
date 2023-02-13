namespace Webscraper_API.Scraper.TCG_Magic.Model.DbModel;

public class Price
{
    public int Id { get; set; }
    public string Usd { get; set; }
    public string Usd_foil { get; set; }
    public string Usd_etched { get; set; }
    public string Eur { get; set; }
    public string Eur_foil { get; set; }
    public string Tix { get; set; }
}
