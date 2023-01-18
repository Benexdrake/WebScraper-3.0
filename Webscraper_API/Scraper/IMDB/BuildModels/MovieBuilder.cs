using Webscraper_API.Scraper.IMDB.Models;

namespace Webscraper_API.Scraper.IMDB.BuildModels
{
    public class MovieBuilder : Builder
    {
        public MovieBuilder(Movie _movie)
        {
            movie = _movie;
        }

        public MovieBuilder Id(string Id)
        {
            movie.Id = Id;
            return this;
        }
        public MovieBuilder Title(string t)
        {
            movie.Title = t;
            return this;
        }

        public MovieBuilder Trailer(string t)
        {
            //movie.TrailerUrl = t;
            return this;
        }

        public MovieBuilder Rating(double r)
        {
            movie.Rating = r;
            return this;
        }

        public MovieBuilder Genres(string g)
        {
            movie.Genres = g;
            return this;
        }

        public MovieBuilder Description(string d)
        {
            movie.Description = d;
            return this;
        }
        public MovieBuilder Url(string u)
        {
            movie.Url = u;
            return this;
        }
        public MovieBuilder ImgUrl(string iU)
        {
            movie.ImgUrl = iU;
            return this;
        }
        public MovieBuilder Director(string d)
        {
            movie.Director = d;
            return this;
        }
        public MovieBuilder Script(string s)
        {
            movie.Script = s;
            return this;
        }
        public MovieBuilder MainCast(string mC)
        {
            movie.MainCast = mC;
            return this;
        }
        public MovieBuilder ReleaseDate(string rD)
        {
            movie.ReleaseDate = rD;
            return this;
        }
        public MovieBuilder OriginCountry(string oC)
        {
            movie.OriginCountry = oC;
            return this;
        }
        public MovieBuilder Budget(string b)
        {
            movie.Budget = b;
            return this;
        }
        public MovieBuilder Runtime(string r)
        {
            movie.Runtime = r;
            return this;
        }
        public MovieBuilder Location(string l)
        {
            movie.Location = l;
            return this;
        }
        public MovieBuilder KnonAs(string k)
        {
            movie.KnownAs = k;
            return this;
        }
        public MovieBuilder ProductionCompanies(string pC)
        {
            movie.ProductionCompanies = pC;
            return this;
        }

        public Movie GetMovie()
        {
            return movie;
        }

        public MovieBuilder NewMovie()
        {
            movie = new Movie();
            return this;
        }
    }
}
