using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper_API.Scraper.IMDB.Models
{
    public class Movie
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        //public string TrailerUrl { get; set; } = string.Empty;
        public string Genres { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Script { get; set; } = string.Empty;
        public string MainCast { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
        public string OriginCountry { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string Runtime { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string KnownAs { get; set; } = string.Empty;
        public string ProductionCompanies { get; set; } = string.Empty;

        // Erscheinungsjahr, Herkunftsland, auch bekannt als, Drehorte, Produktionsfirmen, Budget, Laufzeit
    }
}
