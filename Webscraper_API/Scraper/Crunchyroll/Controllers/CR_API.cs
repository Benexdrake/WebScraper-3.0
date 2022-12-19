

namespace Webscraper_API.Scraper.Crunchyroll.Controllers;

public class CR_API : ICR_API
{
    private readonly IServiceProvider _service;
    private readonly Browser _browser;

    public CR_API(IServiceProvider service)
    {
        _service = service;
        _browser = service.GetRequiredService<Browser>();
    }

    #region Update: Full and Simulcast
    public async Task<string[]> GetAllAnimeUrlsAsync()
    {
        string url = "https://www.crunchyroll.com/de/videos/alphabetical?group=all";
        List<string> urls = new();
        _browser.WebDriver.Navigate().GoToUrl(url);

        await Task.Delay(1000);

        int end = 4000;

        for (int i = 0; i < end; i += 50)
        {
            var page = _browser.WebDriver.PageSource;

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var main = Helper.FindNodesByDocument(doc, "div", "class", "ReactVirtualized__Grid__innerScrollContainer").Result.FirstOrDefault();

            var list = Helper.FindNodesByNode(main, "a", "class", "horizontal-card-static__link--").Result;

            foreach (var item in list)
            {
                var newurl = item.OuterHtml.Split('"')[7];

                urls.Add("https://www.crunchyroll.com" + newurl);
            }
            var percent = Helper.Percent(i, end);
            Log.Logger.Information($"URLs found: {percent}%/100%");
            _browser.WebDriver.ExecuteScript($"window.scrollBy(0, {i});");
            await Task.Delay(2000);
        }
        var dis = urls.Distinct().ToList();
        return dis.ToArray();
    }

    public async Task<string[]> GetSimulcastUpdateUrlsAsync()
    {
        string url = "https://www.crunchyroll.com/de/simulcasts/seasons";
        List<string> urls = new();
        _browser.WebDriver.Navigate().GoToUrl(url);

        var doc = _browser.GetPageDocument(url, 5000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "erc-browse-collection").Result.FirstOrDefault();

        var browsecards = Helper.FindNodesByNode(main, "div", "class", "browse-card-static--UqkrO").Result;

        foreach (var item in browsecards)
        {
            var split = item.InnerHtml.Split('"');

            var u = "https://www.crunchyroll.com" + split[7];
            urls.Add(u);
        }
        return urls.ToArray();
    }
    #endregion

    public async Task<Anime> GetAnimeByUrlAsync(string url, int time)
    {
        var doc = _browser.GetPageDocument(url, time).Result;
        var builder = new BuildModels.Builder();

        var main = Helper.FindNodesByDocument(doc, "div", "class", "erc-series-hero").Result.FirstOrDefault();

        if (main is not null)
        {
            var anime = builder
                        .a
                        .NewAnime()
                        .ID(GetId(url))
                        .Name(GetName(main))
                        .Description(GetDescription(main))
                        .Url(url)
                        .Image(GetImage(doc))
                        .Rating(GetRating(main))
                        .Tags(GetTags(main))
                        .Publisher(GetPublisher(main))
                        .GetAnime();

            return anime;
        }
        return null;
    }


    public async Task<Anime_Episodes> GetAnimewithEpisodes(string url, int time)
    {
        var doc = _browser.GetPageDocument(url, time).Result;
        var builder = new BuildModels.Builder();

        var main = Helper.FindNodesByDocument(doc, "div", "class", "erc-series-hero").Result.FirstOrDefault();

        if (main is not null)
        {
            var episodes = GetEpisodes().Result;
            var anime = builder
                        .a
                        .NewAnime()
                        .ID(GetId(url))
                        .Name(GetName(main))
                        .Description(GetDescription(main))
                        .Url(url)
                        .Image(GetImage(doc))
                        .Rating(GetRating(main))
                        .Tags(GetTags(main))
                        .Publisher(GetPublisher(main))
                        .Episodes(episodes.Length)
                        .GetAnime();

            

            return new Anime_Episodes(anime, episodes);
        }
        return null;
    }


    public async Task<Episode[]> GetEpisodes()
    {
        List<Episode> episodesList = new();

        // Cookies akzeptieren

        var cookie = _browser.WebDriver.FindElements(By.ClassName("evidon-banner-acceptbutton")).FirstOrDefault();
        if (cookie is not null) 
        {
            cookie.Click();
        }
        await Task.Delay(3000);

        // Bin auf der Seite _browser.Webdriver

        var test = _browser.WebDriver.FindElements(By.ClassName("erc-seasons-select")).FirstOrDefault();
        if(test is not null)
        {
            test.Click();

            await Task.Delay(2000);

            // Sammeln der Seasons

            var seasons = _browser.WebDriver.FindElements(By.ClassName("select-content__option--gq8Uo"));

            test.Click();
            await Task.Delay(3000);
            for (int i = 0; i < seasons.Count; i++)
            {
                // Klicken auf Season List Button
                var t = _browser.WebDriver.FindElements(By.ClassName("erc-seasons-select")).FirstOrDefault();
                if(t is not null)
                    t.Click();

                await Task.Delay(2000);
                // Auswahl der Season
                var s = _browser.WebDriver.FindElements(By.ClassName("select-content__option--gq8Uo"));
                if(s.Count> 0)
                    s[i].Click();
                await Task.Delay(1000);

                var episodes = GetEpisodesperSeason().Result;
                episodesList.AddRange(episodes);
                

                await Task.Delay(2000);
            }
        }
        else
        {
            await Task.Delay(1000);
            var episodes = GetEpisodesperSeason().Result;
            episodesList.AddRange(episodes);
        }
        return episodesList.ToArray();
    }

    #region Private Get Methods
    private async Task<Episode[]> GetEpisodesperSeason()
    {
        string page = _browser.ReplaceString(_browser.WebDriver.PageSource);
        var doc = new HtmlDocument();
        doc.LoadHtml(page);

        var main = Helper.FindNodesByDocument(doc,"div","class", "erc-season-with-navigation").Result.FirstOrDefault();
        if(main is not null)
        {
            var episodeList = new List<Episode>();

            var cards = Helper.FindNodesByNode(main, "a", "class", "playable-card-static__thumbnail-wrapper--k9kM5").Result;

            var seasonName = Helper.FindNodesByNode(main, "h4", "class", "text--gq6o- text--is-semibold--AHOYN text--is-xl---ywR-").Result.FirstOrDefault();
            //var seasonName = Helper.FindNodesByNode(main, "h4", "class", "text--gq6o- text--is-semibold--AHOYN text--is-xl---ywR-").Result.FirstOrDefault();

            foreach (var card in cards)
            {
                var episode = new Episode();

                var split = card.ParentNode.InnerHtml.Split('"');

                var time = Helper.FindNodesByNode(card, "div", "class", "text--gq6o- text--is-semibold--AHOYN text--is-m--pqiL- playable-thumbnail__duration--p-Ldq").Result.FirstOrDefault();

                var animeId = _browser.WebDriver.Url.Split('/')[5];

                var id = split[7].Split("/")[3];

                episode.Id = id;
                episode.AnimeId = animeId;
                episode.Title = split[5];
                episode.Url = "https://www.crunchyroll.com" + split[7];
                episode.ImageUrl= split[37];
                episode.Time = time.InnerHtml;
                episode.SeasonName = seasonName.InnerText;
                // Not implented because, it has to load every Episode
                episode.ReleaseDate = "";
                episode.Description= "";

                episodeList.Add(episode);
            }
            return episodeList.ToArray();
        }
        return null;
    }




    private string GetId(string url)
    {
        var id = url.Split('/')[5];
        return id;
    }

    // X
    private string GetName(HtmlNode node)
    {
        var name = Helper.FindNodesByNode(node, "div", "class", "hero-heading-line").Result.FirstOrDefault();
        if (name != null)
        {
            return name.InnerText;
        }
        return "No Name";
    }

    // X
    private string GetImage(HtmlDocument doc)
    {
        var img = Helper.FindNodesByDocument(doc, "img", "alt", "Serien-Hintergrund").Result.FirstOrDefault();
        if (img != null)
        {
            return img.OuterHtml.Split('"')[3];
        }
        return "No Picture";
    }

    private string GetDescription(HtmlNode node)
    {
        var description = Helper.FindNodesByNode(node, "p", "class", "expandable-section__text").Result.FirstOrDefault();
        if (description != null)
            return description.InnerText;

        return "No Description";
    }

    // X
    private string GetTags(HtmlNode node)
    {
        var info = Helper.FindNodesByNode(node, "div", "class", "genres").Result.FirstOrDefault();

        if (info != null)
        {
            var tags = Helper.FindNodesByNode(info, "div", "class", "genre-badge").Result;

            string tag = string.Empty;

            if (tags.Count > 0 || tags != null)
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    if (i < tags.Count - 1)
                        tag += tags[i].InnerText + ", ";
                    else
                        tag += tags[i].InnerText;
                }
                return tag;
            }
        }
        return "";
    }
    // X
    private string GetPublisher(HtmlNode node)
    {
        var details = Helper.FindNodesByNode(node, "div", "class", "show-details-table").Result.FirstOrDefault();
        if (details != null)
        {
            var publisher = Helper.FindNodesByNode(details, "h5", "class", "text--gq6o- text--is-m--pqiL-").Result.FirstOrDefault();
            if (publisher != null)
                return publisher.InnerText;
        }

        return "";
    }

    // X
    private double GetRating(HtmlNode node)
    {
        var rating = Helper.FindNodesByNode(node, "span", "class", "star-rating-average-data__label").Result.FirstOrDefault();

        if (rating != null)
        {
            var d = double.Parse(rating.InnerText.Substring(0, 3).Replace(".",","));
            return d;
        }
        return 0;
    }

    // X
    //private int GetEpisodes(HtmlNode node)
    //{
    //    var episodes = Helper.FindNodesByNode(node, "div", "class", "meta-tags").Result.FirstOrDefault();
    //    //var episodes = Helper.FindNodesByDocument(doc, "div", "class", "erc-playable-collection").Result.FirstOrDefault();
    //    //var list = Helper.FindNodesByNode(episodes, "div", "class", "playable-card-static__body--CmmkB").Result;

    //    //return list.Count;
    //    if (episodes != null)
    //        return int.Parse(episodes.InnerText.Split(" ")[0].Replace(".", ""));
    //    return 0;
    //}

    
    #endregion
}
