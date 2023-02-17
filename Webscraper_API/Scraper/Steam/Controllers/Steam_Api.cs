using OpenQA.Selenium.DevTools.V105.Cast;
using Webscraper_API.Scraper.Apple.Iphone.Models;
using Webscraper_API.Scraper.Steam.Models;

namespace Webscraper_API.Scraper.Steam.Controllers;
public class Steam_Api : ISteam_Api
{
    private readonly Browser _browser;

    public Steam_Api(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }

    public async Task<string[]> GetGameUrls(int category)
    {
        var urls = new List<string>();
        string url = "https://store.steampowered.com";
        var doc = _browser.GetPageDocument(url, 1000).Result;
        var main = Helper.FindNodesByDocument(doc, "div", "class", "home_tabs_content").Result.FirstOrDefault();
        if(main is not null && category < 4 && category >= 0)
        {
            var newItems = Helper.FindNodesByNode(main, "div", "id", "tab_newreleases_content").Result.FirstOrDefault();
            var topsellerItems = Helper.FindNodesByNode(main, "div", "id", "tab_topsellers_content").Result.FirstOrDefault();
            var releaseItems = Helper.FindNodesByNode(main, "div", "id", "tab_upcoming_content").Result.FirstOrDefault();
            var discountItems = Helper.FindNodesByNode(main, "div", "id", "tab_specials_content").Result.FirstOrDefault();

            var tabItems = new List<HtmlNode>();

            switch(category)
            {
                case 0:
                    tabItems = Helper.FindNodesByNode(newItems, "a", "class", "tab_item").Result;
                    break;
                case 1:
                    tabItems = Helper.FindNodesByNode(topsellerItems, "a", "class", "tab_item").Result;
                    break;
                case 2:
                    tabItems = Helper.FindNodesByNode(releaseItems, "a", "class", "tab_item").Result;
                    break;
                case 3:
                    tabItems = Helper.FindNodesByNode(discountItems, "a", "class", "tab_item").Result;
                    break;
            }

            foreach (var item in tabItems)
            {
                var split = item.OuterHtml.Split('"');
                urls.Add(split[1].Split('?')[0]);
            }
        }
        return urls.ToArray();
    }


    public async Task<Game> GetSteamGame(string url)
    {
        var doc = _browser.GetPageDocument(url, 1000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "tablet_grid").Result.FirstOrDefault();
        
        if (main is not null)
        {
            var game = new Game();

            // Game ID
            game.Id = url.Split('/')[4];
            var name = Helper.FindNodesByNode(main, "div", "class", "apphub_AppName").Result.FirstOrDefault();
            game.Title = name.InnerText.Trim();

            // Erster Block
            var gameMediaSummary = Helper.FindNodesByNode(main, "div", "class", "block game_media_and_summary_ctn").Result.FirstOrDefault();

            // Review
            var reviewBlock = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "user_reviews").Result.FirstOrDefault();

            var review = Helper.FindNodesByNode(reviewBlock, "span", "class", "game_review_summary").Result;
            var reviewCount = Helper.FindNodesByNode(reviewBlock, "span", "class", "responsive_hidden").Result; // 0 2
            if(reviewCount.Count > 0)
                reviewCount.RemoveAt(1);
            if(reviewCount.Count > 2)
                reviewCount.RemoveAt(2);

            var reviews = new List<Review>();

            for (int i = 0; i < review.Count; i++)
            {
                var rev = new Review();
                if (review[i].InnerText.Trim().Contains("positiv"))
                    rev.isPositiv = true;
                var count = reviewCount[i].InnerText.Replace("(","").Replace(")", "").Replace(",", "").Replace("--","00").Trim();
                rev.ReviewCount = int.Parse(count);
                reviews.Add(rev);
            }
            game.Review = reviews.ToArray();
            
            // Image Url
            var imageNode = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "game_header_image").Result.FirstOrDefault();
            var imageSplit = imageNode.InnerHtml.Split('"');
            game.GameImageUrl = imageSplit[3].Split('?')[0];

            // Screenshots and Gameplay Videos
            var highlightBlock = Helper.FindNodesByNode(gameMediaSummary, "div", "id", "highlight_player_area").Result.FirstOrDefault();

            var highlightMovieList = Helper.FindNodesByNode(highlightBlock, "div", "class", "highlight_player_item highlight_movie").Result;
            var highlightImageList = Helper.FindNodesByNode(highlightBlock, "div", "class", "highlight_player_item highlight_screenshot").Result;

            var videoList = new List<Video>();
            var imageList = new List<string>();
            foreach (var m in highlightMovieList)
            {
                var split = m.OuterHtml.Split('"'); // 15 video 17 thumbnail
                videoList.Add(new Video() { VideoUrl= split[15].Split('?')[0], ThumbnailUrl = split[17].Split('?')[0] });
            }
            foreach(var i in highlightImageList)
            {
                var split = i.OuterHtml.Split('"'); // Image Url 15
                imageList.Add(split[15].Split('?')[0]);
                Console.WriteLine();
            }
            game.GameplayVideos = videoList.ToArray();
            game.Screenshots = imageList.ToArray();

            // Kurze Beschreibung
            var gameDescriptionSnippet = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "game_description_snippet").Result.FirstOrDefault();
            game.DescriptionSnippet = gameDescriptionSnippet.InnerText.Trim();
            // Release Date
            var releaseDateNode = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "date").Result[1];
            game.ReleaseDate = releaseDateNode.InnerText.Trim();

            // Entwickler Team / Publisher
            var devPublisher = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "dev_row").Result; // 0 = Dev 1 = Publisher
            game.DevTeam = devPublisher[0].InnerText.Replace("Entwickler:","").Trim();
            game.Publisher = devPublisher[1].InnerText.Replace("Publisher:", "").Trim();

            // Tags
            var tagsnodes = Helper.FindNodesByNode(gameMediaSummary, "a", "class", "app_tag").Result;
            var tags = new List<string>();
            foreach (var tag in tagsnodes)
            {
                tags.Add(tag.InnerText.Trim());
            }
            game.Tags = tags.ToArray();


            // Zweiter Block
            // --------------------------------------------------------------------------------------------------------------------------------------
            var pageContent = Helper.FindNodesByNode(main, "div", "class", "page_content").Result.FirstOrDefault();


            // Price Block
            var priceBlock = Helper.FindNodesByNode(pageContent, "div", "class", "game_purchase_action").Result;

            // Ask if there is a Discount Tag like div class discount_prices

            if(priceBlock.Count> 0)
            {
                var discountNode = Helper.FindNodesByNode(priceBlock[0], "div", "class", "discount_prices").Result.FirstOrDefault();
                if (!priceBlock[0].InnerHtml.Contains("Kostenlos"))
                {

                    if(discountNode is null && priceBlock.Count > 2)
                        discountNode = Helper.FindNodesByNode(priceBlock[2], "div", "class", "discount_prices").Result.FirstOrDefault();
                    else if (discountNode is null && priceBlock.Count > 1)
                        discountNode = Helper.FindNodesByNode(priceBlock[1], "div", "class", "discount_prices").Result.FirstOrDefault();
                }

                if (discountNode is not null)
                {
                    // Original Price
                    var originalPrice = Helper.FindNodesByNode(discountNode, "div", "class", "discount_original_price").Result.FirstOrDefault();

                    // Discount Price
                    var finalPrice = Helper.FindNodesByNode(discountNode, "div", "class", "discount_final_price").Result.FirstOrDefault();

                    string op = originalPrice.InnerText.Replace("€", "");
                    string fp = finalPrice.InnerText.Replace("€", "");

                    game.Price = double.Parse(op);
                    game.DiscountPrice = double.Parse(fp);
                }
                else
                {
                    // Price
                    var price = Helper.FindNodesByNode(priceBlock[0], "div", "class", "game_purchase_price").Result.FirstOrDefault();
                    if(price is null)
                    {
                        if(priceBlock.Count > 2)
                            price = Helper.FindNodesByNode(priceBlock[2], "div", "class", "game_purchase_price").Result.FirstOrDefault();
                    }
                    if(price is not null)
                    {
                        string p = price.InnerText.Replace("€", "");
                        game.IsFreeToPlay = !double.TryParse(p, out var PRICE);
                        game.Price = PRICE;
                    }
                }
            }

            // Category Block
            var categoryBlock = Helper.FindNodesByNode(main, "div", "class", "game_area_features_list").Result.FirstOrDefault();
            var gameAreaDetailsSpecs = Helper.FindNodesByNode(categoryBlock, "a", "class", "game_area_details_specs_ctn").Result;
            var detailSpecs = new List<string>();
            foreach (var item in gameAreaDetailsSpecs)
            {
                detailSpecs.Add(item.InnerText.Trim());
            }
            game.GameFeatures= detailSpecs.ToArray();

            // Language Block
            var languageBlock = Helper.FindNodesByNode(main, "table", "class", "game_language_options").Result.FirstOrDefault();
            var languageNodes = Helper.FindNodesByNode(languageBlock, "tr", "class", "").Result;
            var languages = new List<Language>();
            for (int i = 1; i < languageNodes.Count; i++)
            {
                if (languageNodes[i].InnerHtml.Contains("Nicht unterstützt"))
                {
                    continue;
                }
                var l = new Language();
                var tds = Helper.FindNodesByNode(languageNodes[i], "td", "class", "").Result;
                l.Name = tds[0].InnerText.Trim();
                if (tds[1].InnerText.Contains("✔"))
                    l.Surface = true;
                if (tds[2].InnerText.Contains("✔"))
                    l.Sound= true;
                if (tds[3].InnerText.Contains("✔"))
                    l.Subtitle = true;
                

                languages.Add(l);
            }
            game.Languages= languages.ToArray();

            // Game Description Block
            var gameDescriptionBlock = Helper.FindNodesByNode(main, "div", "class", "game_area_description").Result;
            foreach (var desc in gameDescriptionBlock)
            {
                if(desc.InnerText.Contains("Über dieses Spiel"))
                    game.Description = desc.InnerText.Replace("Über dieses Spiel", "").Trim();

            }
            // System Requierments
            var systemRequiermentBlock = Helper.FindNodesByNode(main, "div", "class", "game_area_sys_req sysreq_content active").Result.FirstOrDefault();
            var systemRequierments = Helper.FindNodesByNode(systemRequiermentBlock, "div", "class", "game_area_sys_req").Result;
            var pcList = new List<PcSpecs>();
            foreach (var sr in systemRequierments)
            {
                var pc = new PcSpecs();
                var list = Helper.FindNodesByNode(sr, "li", "", "").Result;
                foreach (var item in list)
                {
                    pc.Information = list[0].InnerText.Trim();
                    if (item.InnerHtml.Contains("Betriebssystem"))
                        pc.OS = item.InnerText.Trim();
                    if (item.InnerHtml.Contains("Prozessor"))
                        pc.CPU = item.InnerText.Trim();
                    if (item.InnerHtml.Contains("Arbeitsspeicher"))
                        pc.RAM = item.InnerText.Trim();
                    if (item.InnerHtml.Contains("Grafik"))
                        pc.GPU = item.InnerText.Trim();
                    if (item.InnerHtml.Contains("Netzwerk"))
                        pc.DirectX = item.InnerText.Trim();
                    if (item.InnerHtml.Contains("Speicherkapazität") || item.InnerHtml.Contains("Speicherplatz"))
                        pc.HardDisk = item.InnerText.Trim();
                    if (item.InnerHtml.Contains("Zusätzliche Anmerkungen"))
                        pc.AdditionalNote = item.InnerText.Trim();
                }

                pcList.Add(pc);
            }
            game.PcSpecs = pcList.ToArray();
            return game;
        }
        return null;
    }
}
