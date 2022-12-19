
using Webscraper_API.Scraper.TCG_Magic.Model;

public class Set
{
    public string _object { get; set; }
    public string Id { get; set; }
    public string Code { get; set; }
    public int Tcgplayer_id { get; set; }
    public string Name { get; set; }
    public string Uri { get; set; }
    public string Scryfall_uri { get; set; }
    public string Search_uri { get; set; }
    public string Released_at { get; set; }
    public string Set_type { get; set; }
    public int Card_count { get; set; }
    public bool Digital { get; set; }
    public bool Nonfoil_only { get; set; }
    public bool Foil_only { get; set; }
    public string Block_code { get; set; }
    public string Block { get; set; }
    public string Icon_svg_uri { get; set; }
    public Card[] Cards { get; set; }
}
