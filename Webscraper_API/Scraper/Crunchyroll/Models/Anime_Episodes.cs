using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper_API.Scraper.Crunchyroll.Models
{
    public class Anime_Episodes
    {
        public Anime_Episodes(Anime anime, Episode[] episodes)
        {
            Anime = anime;
            Episodes = episodes;
        }

        public Anime Anime { get; set; }
        public Episode[] Episodes { get; set; }
    }
}
