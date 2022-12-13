using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webscraper_API.Scraper.IMDB.Models;

namespace Webscraper_API.Scraper.IMDB.BuildModels
{
    public class Builder
    {
        protected Movie movie = new Movie();

        public MovieBuilder m => new(movie);
    }
}
