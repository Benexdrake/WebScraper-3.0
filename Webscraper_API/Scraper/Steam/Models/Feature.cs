using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper_API.Scraper.Steam.Models
{
    public class Feature
    {
        public int Id { get; set; }
        public string F { get; set; } = string.Empty;
    }
}
