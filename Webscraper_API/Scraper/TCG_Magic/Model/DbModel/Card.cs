namespace Webscraper_API.Scraper.TCG_Magic.Model.DbModel;

public class Card
{
    public string Id { get; set; }
    public string Oracle_id { get; set; }
    public MultiverseID[] Multiverse_ids { get; set; }
    public int Tcgplayer_id { get; set; }
    public int Cardmarket_id { get; set; }
    public string Name { get; set; }
    public string Lang { get; set; }
    public string Released_at { get; set; }
    public string Uri { get; set; }
    public string Scryfall_uri { get; set; }
    public string Layout { get; set; }
    public bool Highres_image { get; set; }
    public string Image_status { get; set; }
    public Image_Uri Image_uri { get; set; }
    public string Mana_cost { get; set; }
    public float Cmc { get; set; }
    public string Type_line { get; set; }
    public string Oracle_text { get; set; }
    public Color[] Colors { get; set; }
    public ColorIdentity[] Color_identity { get; set; }
    public Keyword[] Keywords { get; set; }
    public Legality Legality { get; set; }
    public Game[] Games { get; set; }
    public bool Reserved { get; set; }
    public bool Foil { get; set; }
    public bool Nonfoil { get; set; }
    public Finish[] Finishes { get; set; }
    public bool Oversized { get; set; }
    public bool Promo { get; set; }
    public bool Reprint { get; set; }
    public bool Variation { get; set; }
    public string Set_id { get; set; }
    public string Set { get; set; }
    public string Set_name { get; set; }
    public string Set_type { get; set; }
    public string Set_uri { get; set; }
    public string Set_search_uri { get; set; }
    public string Scryfall_set_uri { get; set; }
    public string Rulings_uri { get; set; }
    public string Prints_search_uri { get; set; }
    public string Collector_number { get; set; }
    public bool Digital { get; set; }
    public string Rarity { get; set; }
    public string Card_back_id { get; set; }
    public string Artist { get; set; }
    public ArtistId[] Artist_ids { get; set; }
    public string Illustration_id { get; set; }
    public string Border_color { get; set; }
    public string Frame { get; set; }
    public bool Full_art { get; set; }
    public bool Textless { get; set; }
    public bool Booster { get; set; }
    public bool Story_spotlight { get; set; }
    public int Edhrec_rank { get; set; }
    public int Penny_rank { get; set; }
    public Price Price { get; set; }
    public Related_Uri Related_uri { get; set; }
    public Purchase_Uri Purchase_uri { get; set; }

}
