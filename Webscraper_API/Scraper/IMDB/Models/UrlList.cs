using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper_API.Scraper.IMDB.Models
{
    public class UrlList
    {
        public int Count { get; set; }
        public List<string> Urls { get; set; } = new List<string>();
    }
}
