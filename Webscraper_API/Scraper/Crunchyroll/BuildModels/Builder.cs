using Webscraper_API.Scraper.Crunchyroll.Models;

namespace Webscraper_API.Scraper.Crunchyroll.BuildModels
{
    public class Builder
    {
        protected Anime anime = new();

        public AnimeBuilder a => new AnimeBuilder(anime);

    }
}
