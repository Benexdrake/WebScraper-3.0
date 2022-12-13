namespace Webscraper_API.Scraper.Crunchyroll.Controllers;

public class CR_API : ICR_API
{
    private readonly IServiceProvider _service;

    public CR_API(IServiceProvider service)
    {
        _service = service;
    }

    #region Get an Anime Url List from https://www.crunchyroll.com/de/videos/anime/alpha?group=all
    public async Task<string[]> GetAnimeUrlList(WebDriver? driver)
    {
        string url = "https://www.crunchyroll.com/de/videos/alphabetical?group=all";
        List<string> urls = new();
        driver.Navigate().GoToUrl(url);

        await Task.Delay(3000);

        int end = 4000;

        for (int i = 0; i < end; i += 50)
        {
            var page = driver.PageSource;

            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var main = FindNodesByDocument(doc, "div", "class", "ReactVirtualized__Grid__innerScrollContainer").Result.FirstOrDefault();

            var list = FindNodesByNode(main, "a", "class", "horizontal-card-static__link--").Result;

            foreach (var item in list)
            {
                var newurl = item.OuterHtml.Split('"')[7];

                urls.Add("https://www.crunchyroll.com" + newurl);
            }
            var percent = Helper.Percent(i, end);
            Log.Logger.Information($"URLs found: {percent}%/100%");
            driver.ExecuteScript($"window.scrollBy(0, {i});");
            await Task.Delay(2000);
        }
        var dis = urls.Distinct().ToList();
        return dis.ToArray();
    }
    #endregion


    public async Task<Anime> GetAnimeByUrlAsync(string url, HtmlDocument doc)
    {
        var builder = new BuildModels.Builder();

        var main = FindNodesByDocument(doc, "div", "class", "erc-series-hero").Result.FirstOrDefault();

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
                        .Episodes(GetEpisodes(main))
                        .LastUpdate()
                        .GetAnime();

            return anime;
        }
        return null;
    }


    public async Task GetEpisode(HtmlDocument doc)
    {
        var main = FindNodesByDocument(doc, "div", "class", "erc-series-hero").Result.FirstOrDefault();


    }



    #region Private Get Methods

    private string GetId(string url)
    {
        var id = url.Split('/')[5];
        return id;
    }

    // X
    private string GetName(HtmlNode node)
    {
        var name = FindNodesByNode(node, "div", "class", "hero-heading-line").Result.FirstOrDefault();
        if (name != null)
        {
            return name.InnerText;
        }
        return "No Name";
    }

    // X
    private string GetImage(HtmlDocument doc)
    {
        var img = FindNodesByDocument(doc, "img", "alt", "Serien-Hintergrund").Result.FirstOrDefault();
        if (img != null)
        {
            return img.OuterHtml.Split('"')[3];
        }
        return "No Picture";
    }

    private string GetDescription(HtmlNode node)
    {
        var description = FindNodesByNode(node, "p", "class", "expandable-section__text").Result.FirstOrDefault();
        if (description != null)
            return description.InnerText;

        return "No Description";
    }

    // X
    private string GetTags(HtmlNode node)
    {
        var info = FindNodesByNode(node, "div", "class", "genres-wrapper").Result.FirstOrDefault();

        if (info != null)
        {
            var tags = FindNodesByNode(info, "div", "class", "genre-badge").Result;

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
        var details = FindNodesByNode(node, "div", "class", "show-details-table").Result.FirstOrDefault();
        if (details != null)
        {
            var publisher = FindNodesByNode(details, "h5", "class", "text--gq6o- text--is-m--pqiL-").Result.FirstOrDefault();
            if (publisher != null)
                return publisher.InnerText;
        }

        return "";
    }

    // X
    private string GetRating(HtmlNode node)
    {
        var rating = FindNodesByNode(node, "span", "class", "star-rating-average-data__label").Result.FirstOrDefault();

        if (rating != null)
        {
            return rating.InnerText.Substring(0, 3);
        }
        return "No Ratings";
    }

    // X
    private int GetEpisodes(HtmlNode node)
    {
        var episodes = FindNodesByNode(node, "div", "class", "meta-tags").Result.FirstOrDefault();
        if (episodes != null)
            return int.Parse(episodes.InnerText.Split(" ")[0].Replace(".", ""));
        return 0;
    }

    private int Procent(int n, int max)
    {
        return (int)Math.Round((double)(100 * n) / max);
    }

    private async Task<List<HtmlNode>> FindNodesByNode(HtmlNode node, string a, string b, string c)
    {
        return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
    }
    private async Task<List<HtmlNode>> FindNodesByDocument(HtmlDocument document, string a, string b, string c)
    {
        return document.DocumentNode.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
    }
    #endregion
}
