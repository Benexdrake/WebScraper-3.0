﻿using Webscraper.Models.Crunchyroll.Models;

namespace Webscraper.Models.Crunchyroll.BuildModels
{
    public class AnimeBuilder : CR_Builder
    {
        public AnimeBuilder(Anime _anime)
        {
            anime = _anime;
        }

        public AnimeBuilder ID(string id)
        {
            anime.Id = id;
            return this;
        }

        public AnimeBuilder Name(string name)
        {
            anime.Name = name;
            return this;
        }
        public AnimeBuilder Description(string desc)
        {
            anime.Description = desc;
            return this;
        }

        public AnimeBuilder Episodes(int episodes)
        {
            anime.Episodes = episodes;
            return this;
        }

        public AnimeBuilder Url(string url)
        {
            anime.Url = url;
            return this;
        }
        public AnimeBuilder Image(string url)
        {
            anime.ImageUrl = url;
            return this;
        }
        public AnimeBuilder Rating(double rating)
        {
            anime.Rating = rating;
            return this;
        }
        public AnimeBuilder Tags(string tag)
        {
            anime.Tags = tag;
            return this;
        }
        public AnimeBuilder Publisher(string pub)
        {
            anime.Publisher = pub;
            return this;
        }

        public Anime GetAnime()
        {
            return anime;
        }

        public AnimeBuilder NewAnime()
        {
            anime = new Anime();
            return this;
        }
    }
}
