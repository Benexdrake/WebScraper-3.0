using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webscraper_API.Scraper.TCG_Magic.Old;

namespace EF_Core_Console.Data
{
    internal class MagicCardsDBContext : DbContext
    {
        public DbSet<MagicCard> MagicCards { get; set; }


        public Image_Uris Image_Uris { get; set; }
        public Legalities Legalities { get; set; }
        public Preview Preview { get; set; }
        public Prices Prices { get; set; }
        public Related_Uris Related_Uris { get; set; }
        public Purchase_Uris Purchase_Uris { get; set; }
    }
}
