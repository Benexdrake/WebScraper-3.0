

using OpenQA.Selenium;
using System;

namespace Webscraper_API.Scraper.Crunchyroll.Controllers;

public class CR_API : ICR_API
{
    private readonly Browser _browser;

    public string Message { get; set; }

    public CR_API(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }

    #region Update: Full and Simulcast
    public async Task<string[]> GetAllAnimeUrlsAsync()
    {

        // New with other Site
        List<string> urls = new();
        Message = "0%/100%";
        string url = "https://www.crunchyroll.com/de/videos/new";
        _browser.WebDriver.Navigate().GoToUrl(url);
        await Task.Delay(5000);

        int end = 5000;


        // browsen bis nach unten in gaaanz schnell
        for (int i = 0; i < end; i+=100)
        {
            var percent = Helper.Percent(i, end);
            Message = $"URLs found: {percent}%/100%";
            Log.Logger.Information(Message);
            _browser.WebDriver.ExecuteScript($"window.scrollBy(0, {i});");
            await Task.Delay(1000);
        }

        var page = _browser.WebDriver.PageSource;

        var doc = new HtmlDocument();
        doc.LoadHtml(page);

        await Task.Delay(5000);

        var main = Helper.FindNodesByDocument(doc, "div", "class", "content-wrapper--MF5LS").Result.FirstOrDefault();

        if(main is not null)
        {
            var collection = Helper.FindNodesByDocument(doc, "div", "class", "erc-browse-cards-collecti").Result;
            // Collection == 3

            foreach (var c in collection)
            {
                var browseCards = Helper.FindNodesByNode(c, "div", "class", "browse-card-static--UqkrO").Result;
                foreach (var card in browseCards)
                {
                    var split = card.InnerHtml.Split('"');

                    var u = "https://www.crunchyroll.com" + split[7];
                    urls.Add(u);
                }
            }
        }
       
        Message = $"URLs found: 100%/100%";
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

    public async Task<string[]> GetDailyUpdateAsync()
    {
        string url = "https://www.crunchyroll.com/de/videos/new";
        List<string> urls = new();
        _browser.WebDriver.Navigate().GoToUrl(url);

        var doc = _browser.GetPageDocument(url, 5000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "browse-collection-wrapper").Result.FirstOrDefault();

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
            var episodes = GetEpisodesAsync().Result.ToList();

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
                        .Episodes(episodes.Count)
                        .GetAnime();

            return new Anime_Episodes(anime, episodes.ToArray());
        }
        return null;
    }

    public async Task<Episode[]> GetEpisodesAsync()
    {
        List<Episode> episodesList = new();

        var cookie = _browser.WebDriver.FindElements(By.ClassName("evidon-banner-acceptbutton")).FirstOrDefault();
        if (cookie is not null)
        {
            cookie.Click();
        }
        await Task.Delay(5000);

        // while schleife bis div hat state-disabled für <
        while (true)
        {
            await Task.Delay(1000);

            var before = _browser.WebDriver.FindElements(By.ClassName("cta-wrapper")).FirstOrDefault();
            if (before is not null)
            {
                var b = before.GetAttribute("class");

                if (b.Contains("state-disabled"))
                    break;
                else
                    before.Click();
            }
            else
                break;
        }

        // while schleife bis div hat state-disabled für >

        while(true)
        {
            await Task.Delay(1200);

            while (true)
            {
                var more = _browser.WebDriver.FindElements(By.ClassName("button--is-type-four--yKPXY")).FirstOrDefault();

                if (more is null)
                    break;
                else
                    more.Click();
                await Task.Delay(1000);
            }

            var episodes = GetEpisodesperSeason().Result;
            episodesList.AddRange(episodes);
            //Console.WriteLine(episodesList.Count);

            var next = _browser.WebDriver.FindElements(By.ClassName("cta-wrapper"))[1];


            if(next is not null)
            {
                var b = next.GetAttribute("class");
                if (b.Contains("state-disabled"))
                    break;
                else
                {
                    next.Click();
                }
            }
        }
        return episodesList.ToArray();
    }

    public async Task<Episode> GetEpisodeDetails(Episode episode)
    {
        string url = episode.Url;

        var doc = _browser.GetPageDocument(url, 2000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "erc-current-media-info").Result.FirstOrDefault();
        if (main is null)
            return episode;

        var date = Helper.FindNodesByNode(main, "p", "class", "text--gq6o- text--is-m--pqiL- release-date").Result.FirstOrDefault();
        if(date is not null)
        {
            var d = date.InnerHtml.Replace("Veröffentlicht am ", "");
            if (!string.IsNullOrWhiteSpace(d))
            {
                var datetime = DateTime.Parse(d).ToString("dd.MM.yyyy");
                episode.ReleaseDate = datetime;
            }
            else
                episode.ReleaseDate = "-";
        }

        var desc = Helper.FindNodesByNode(main, "p", "class", "text--gq6o- text--is-l--iccTo expandable-section__text---00oG").Result.FirstOrDefault();
        if (desc is not null)
        {
            episode.Description = desc.InnerText;
            if (string.IsNullOrWhiteSpace(desc.InnerText))
                episode.Description = "-";
        }
        else
            episode.Description = "-";

        return episode;
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

    private string GetName(HtmlNode node)
    {
        var name = Helper.FindNodesByNode(node, "div", "class", "hero-heading-line").Result.FirstOrDefault();
        if (name != null)
        {
            return name.InnerText;
        }
        return "No Name";
    }

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

    private double GetRating(HtmlNode node)
    {
        var rating = Helper.FindNodesByNode(node, "span", "class", "star-rating-average-data__label").Result.FirstOrDefault();

        if (rating != null)
        {
            var number = double.TryParse(rating.InnerText.Substring(0, 3).Replace(".", ","), out double d);
            if (number)
                return d;
            return 0;
        }
        return 0;
    }
    #endregion
}
