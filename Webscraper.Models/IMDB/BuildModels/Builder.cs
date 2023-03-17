using Webscraper.Models.IMDB.Models;

namespace Webscraper.Models.IMDB.BuildModels
{
    public class Builder
    {
        protected Movie movie = new Movie();

        public MovieBuilder m => new(movie);
    }
}
