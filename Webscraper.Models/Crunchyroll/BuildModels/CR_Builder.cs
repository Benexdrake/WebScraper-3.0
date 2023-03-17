using Webscraper.Models.Crunchyroll.Models;

namespace Webscraper.Models.Crunchyroll.BuildModels
{
    public class CR_Builder
    {
        protected Anime anime = new();

        public AnimeBuilder a => new AnimeBuilder(anime);

    }
}
