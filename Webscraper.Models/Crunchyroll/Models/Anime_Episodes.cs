namespace Webscraper.Models.Crunchyroll.Models
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
