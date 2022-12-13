using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper_API.Scraper.TCG_Pokemon.Models
{
    public class PokemonCard
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string KP { get; set; }
        public string Element { get; set; } = string.Empty;
        public Skills[] Skills { get; set; }
        public string Weakness { get; set; } = string.Empty;
        public string Resistance { get; set; } = string.Empty;
        public string Retreat { get; set; } = string.Empty;
        public string Expansion { get; set; } = string.Empty;
        public string Illustrator { get; set; } = string.Empty;
        public string EvolvesName { get; set; } = string.Empty;
    }
}
