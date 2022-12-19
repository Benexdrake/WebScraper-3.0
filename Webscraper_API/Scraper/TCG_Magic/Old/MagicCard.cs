using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper_API.Scraper.TCG_Magic.Old
{
    public class MagicCard
    {
        public string Object { get; set; }
        public string Id { get; set; }
        public string Oracle_Id { get; set; }
        public string[] Multiverse_Ids { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        public string Released_At { get; set; }
        public string Uri { get; set; }
        public string Scryfall_Uri { get; set; }
        public string Layout { get; set; }
        public bool Highres_Image { get; set; }
        public string Image_Status { get; set; }
        public Image_Uris Image_Uris { get; set; }
        public string Mana_Cost { get; set; }
        public double Cmc { get; set; }
        public string Type_Line { get; set; }
        public string Oracle_Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string[] Colors { get; set; }
        public string[] Color_Identity { get; set; }
        public string[] Keywords { get; set; }
        public Legalities Legalities { get; set; }
        public string[] Games { get; set; }
        public bool Reserved { get; set; }
        public bool Foil { get; set; }
        public bool NonFoil { get; set; }
        public string[] Finishes { get; set; }
        public bool Oversized { get; set; }
        public bool Promo { get; set; }
        public bool Reprint { get; set; }
        public bool Variation { get; set; }
        public string Set_Id { get; set; }
        public string Set { get; set; }
        public string Set_Name { get; set; }
        public string Set_Type { get; set; }
        public string Set_Uri { get; set; }
        public string Set_Search_Uri { get; set; }
        public string Scryfall_Set_Uri { get; set; }
        public string Rulings_Uri { get; set; }
        public string Prints_Search_Uri { get; set; }
        public string Collector_Number { get; set; }
        public bool Digital { get; set; }
        public string Rarity { get; set; }
        public string Watermark { get; set; }
        public string Card_Back_Id { get; set; }
        public string Artist { get; set; }
        public string[] Artist_Ids { get; set; }
        public string Illustration_Id { get; set; }
        public string Border_Color { get; set; }
        public string Frame { get; set; }
        public string[] Frame_Effects { get; set; }
        public string Security_stamp { get; set; }
        public bool Full_Art { get; set; }
        public bool Textless { get; set; }
        public bool Booster { get; set; }
        public bool Story_Spotlight { get; set; }
        public Preview Preview { get; set; }
        public Prices Prices { get; set; }
        public Related_Uris Related_Uris { get; set; }
        public Purchase_Uris Purchase_Uris { get; set; }
    }
}
