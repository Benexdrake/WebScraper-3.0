using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Webscraper.API.Interfaces;

namespace Webscraper.API.Scraper.Steam.Controllers;
public class Steam_Api : ISteam_Api
{
    private readonly Browser _browser;
    private readonly HttpClient _http;
    public string Message { get; set; }

    public Steam_Api(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
        _http = service.GetRequiredService<HttpClient>();
    }

    #region Game Urls
    public async Task<App[]> GetAllGameIds()
    {
        var root = _http.GetFromJsonAsync<Models.Steam.Models.Rootobject>("https://api.steampowered.com/ISteamApps/GetAppList/v2/").Result;

        return root.applist.apps;
    }

    public async Task<string[]> GetGameUrlsFromWishlist(string url)
    {
        List<string> gameUrls = new List<string>();
        var doc = _browser.GetPageDocument(url, 2000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "id", "wishlist_ctn").Result.FirstOrDefault();
        if (main is not null)
        {

            var height = _browser.WebDriver.ExecuteScript($"return document.body.scrollHeight");

            int end = int.Parse(height.ToString());

            // browsen bis nach unten in gaaanz schnell
            for (int i = 0; i < end; i += 1000)
            {

                string page = _browser.WebDriver.PageSource;
                doc = new HtmlDocument();
                doc.LoadHtml(page);

                var wishlistRow = Helper.FindNodesByDocument(doc, "div", "class", "wishlist_row").Result;
                foreach (var row in wishlistRow)
                {
                    var link = Helper.FindNodesByNode(row, "div", "class", "content").Result.FirstOrDefault();
                    var split1 = link.InnerHtml.Split('"')[1].Split("/");
                    gameUrls.Add("https://store.steampowered.com/app/" + split1[4]);
                }

                var percent = Helper.Percent(i, end);
                Message = $"URLs found: {percent}% / 100%";
                _browser.WebDriver.ExecuteScript($"window.scrollBy(0, 1000);");
                await Task.Delay(1000);
            }
            Console.WriteLine(gameUrls.Count);
            gameUrls = gameUrls.Distinct().ToList();
            Console.WriteLine(gameUrls.Count);
            Message = $"URLs found: 100% / 100%";

        }
        return gameUrls.ToArray();
    }

    public async Task<string[]> GetGameUrls(int category)
    {
        var urls = new List<string>();
        string url = "https://store.steampowered.com";
        var doc = _browser.GetPageDocument(url, 1000).Result;
        var main = Helper.FindNodesByDocument(doc, "div", "class", "home_tabs_content").Result.FirstOrDefault();
        if (main is not null && category < 4 && category >= 0)
        {
            var newItems = Helper.FindNodesByNode(main, "div", "id", "tab_newreleases_content").Result.FirstOrDefault();
            var topsellerItems = Helper.FindNodesByNode(main, "div", "id", "tab_topsellers_content").Result.FirstOrDefault();
            var releaseItems = Helper.FindNodesByNode(main, "div", "id", "tab_upcoming_content").Result.FirstOrDefault();
            var discountItems = Helper.FindNodesByNode(main, "div", "id", "tab_specials_content").Result.FirstOrDefault();

            var tabItems = new List<HtmlNode>();

            switch (category)
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
    #endregion

    #region Get a Steam Game
    public async Task<SteamGame> GetSteamGame(string url)
    {
        try
        {
            var doc = _browser.GetPageDocument(url, 1000).Result;

            var main = Helper.FindNodesByDocument(doc, "div", "class", "tablet_grid").Result.FirstOrDefault();

            if (main is not null)
            {
                var game = new SteamGame();

                // Game ID
                game._id = url.Split('/')[4];
                var name = Helper.FindNodesByNode(main, "div", "class", "apphub_AppName").Result.FirstOrDefault();
                game.Title = name.InnerText.Trim();

                var bread = Helper.FindNodesByNode(main, "div", "class", "breadcrumbs").Result.FirstOrDefault();
                if (bread is not null)
                    if (bread.InnerText.Contains("Zusatzinhalte"))
                        game.IsDLC = true;

                var earlyAccess = Helper.FindNodesByNode(main, "div", "class", "early_access_header").Result.FirstOrDefault();
                if (earlyAccess is not null)
                    game.IsEarlyAccess = true;
                // Erster Block
                var gameMediaSummary = Helper.FindNodesByNode(main, "div", "class", "block game_media_and_summary_ctn").Result.FirstOrDefault();


                // Review
                var reviewBlock = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "user_reviews").Result.FirstOrDefault();

                var review = Helper.FindNodesByNode(reviewBlock, "span", "class", "game_review_summary").Result;
                var reviewCount = Helper.FindNodesByNode(reviewBlock, "span", "class", "responsive_hidden").Result; // 0 2
                if (reviewCount.Count > 1)
                    reviewCount.RemoveAt(1);
                if (reviewCount.Count > 2)
                    reviewCount.RemoveAt(2);

                var reviews = new List<Review>();

                for (int i = 0; i < review.Count; i++)
                {
                    var rev = new Review();
                    if (review[i].InnerText.Trim().Contains("positiv"))
                        rev.isPositiv = true;
                    var count = reviewCount[i].InnerText.Replace("(", "").Replace(")", "").Replace(",", "").Replace("--", "00").Trim();
                    var isNumber = int.TryParse(count, out var number);
                    rev.ReviewCount = number;
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
                    videoList.Add(new Video() { VideoUrl = split[15].Split('?')[0], ThumbnailUrl = split[17].Split('?')[0] });
                }
                foreach (var i in highlightImageList)
                {
                    var split = i.OuterHtml.Split('"'); // Image Url 15
                    imageList.Add(split[15].Split('?')[0]);
                }
                game.GameplayVideos = videoList.ToArray();
                game.Screenshots = imageList.ToArray();

                // Kurze Beschreibung
                var gameDescriptionSnippet = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "game_description_snippet").Result.FirstOrDefault();
                if (gameDescriptionSnippet is not null)
                    game.DescriptionSnippet = gameDescriptionSnippet.InnerText.Trim();

                // Release Date
                var releaseDateNode = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "date").Result;
                if (releaseDateNode.Count > 1)
                    game.ReleaseDate = releaseDateNode[1].InnerText.Trim();

                // Entwickler Team / Publisher
                var devPublisher = Helper.FindNodesByNode(gameMediaSummary, "div", "class", "dev_row").Result; // 0 = Dev 1 = Publisher
                if (devPublisher is not null)
                {
                    foreach (var dp in devPublisher)
                    {
                        if (dp.InnerText.Contains("Entwickler"))
                            game.DevTeam = dp.InnerText.Replace("Entwickler:", "").Trim();
                        else if (dp.InnerText.Contains("Publisher"))
                            game.Publisher = dp.InnerText.Replace("Publisher:", "").Replace("+", "").Trim();
                    }
                }

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
                var priceBlock = Helper.FindNodesByNode(pageContent, "div", "class", "game_area_purchase").Result.FirstOrDefault();

                var priceWrappers = Helper.FindNodesByNode(priceBlock, "div", "class", "game_area_purchase_game_wrapper").Result.ToList();

                List<Price> prices = new List<Price>();

                foreach (var pw in priceWrappers)
                {
                    var h1 = Helper.FindNodesByNode(pw, "h1", "", "").Result.FirstOrDefault();

                    var originalPrice = Helper.FindNodesByNode(pw, "div", "class", "game_purchase_price price").Result.FirstOrDefault();
                    if (originalPrice is null)
                    {
                        originalPrice = Helper.FindNodesByNode(pw, "div", "class", "discount_original_price").Result.FirstOrDefault();
                        if (originalPrice is null)
                            originalPrice = Helper.FindNodesByNode(pw, "div", "class", "discount_final_price your_price").Result.FirstOrDefault();
                    }
                    if (originalPrice is not null)
                    {
                        var discountPrice = Helper.FindNodesByNode(pw, "div", "class", "discount_final_price").Result.FirstOrDefault();
                        double dp = 0;
                        if (discountPrice is not null)
                            dp = double.Parse(discountPrice.InnerText.Replace("€", "").Replace("Ihr Preis:", "").Replace("--", "00").Replace(" ", ""));

                        var isNotFree = double.TryParse(originalPrice.InnerText.Replace("€", "").Replace("Ihr Preis:", "").Replace("--", "00").Replace(" ", ""), out double op);

                        if (isNotFree)
                        {
                            prices.Add(new Price()
                            {
                                OriginalPrice = op,
                                DiscountPrice = dp,
                                IsFree = !isNotFree,
                                Title = h1.InnerText.Replace("BÜNDEL", "").Replace("(?)", "").Replace("kaufen", "").Trim()
                            });
                        }
                        else
                            prices.Add(new Price()
                            {
                                OriginalPrice = 0,
                                DiscountPrice = 0,
                                IsFree = isNotFree,
                                Title = h1.InnerText
                            });
                    }
                }

                // Category Block
                var categoryBlock = Helper.FindNodesByNode(main, "div", "class", "game_area_features_list").Result.FirstOrDefault();
                if (categoryBlock is not null)
                {
                    var gameAreaDetailsSpecs = Helper.FindNodesByNode(categoryBlock, "a", "class", "game_area_details_specs_ctn").Result;

                    var detailSpecs = new List<string>();
                    foreach (var item in gameAreaDetailsSpecs)
                    {
                        detailSpecs.Add(item.InnerText.Trim());
                    }
                    game.GameFeatures = detailSpecs.ToArray();
                }

                // Language Block
                var languageBlock = Helper.FindNodesByNode(main, "table", "class", "game_language_options").Result.FirstOrDefault();
                if (languageBlock is not null)
                {
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
                            l.Sound = true;
                        if (tds[3].InnerText.Contains("✔"))
                            l.Subtitle = true;


                        languages.Add(l);
                    }
                    game.Languages = languages.ToArray();
                }

                // Game Description Block
                var gameDescriptionBlock = Helper.FindNodesByNode(main, "div", "class", "game_area_description").Result;
                foreach (var desc in gameDescriptionBlock)
                {
                    if (desc.InnerText.Contains("Über dieses Spiel"))
                        game.Description = desc.InnerText.Replace("Über dieses Spiel", "").Trim();

                }
                // System Requierments
                var systemRequiermentBlock = Helper.FindNodesByNode(main, "div", "class", "game_area_sys_req sysreq_content active").Result.FirstOrDefault();
                if (systemRequiermentBlock is not null)
                {
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
                }
                game.Url = url;
                game.Prices = prices.ToArray();

                return game;
            }
        }
        catch (Exception err)
        {
            Log.Logger.Error(err.Message);
            throw;
        }
        return null;
    }
    #endregion

    // Users

    // Get a User

    public async Task<SteamUser> GetUser(string url)
    {
        var split = url.Split('/');
        string newUrl = string.Empty;
        string ID = string.Empty;
        newUrl = "https://steamcommunity.com/profiles/" + split[4];
        ID = split[4];
        //if(url.Contains("https://steamcommunity.com/id/"))
        //{
        //    var split = url.Split("/");
        //    var steamID = TranslateSteamID(split[4]);
        //   
        //    Console.WriteLine();
        //}
        //else
        //{
        //}
        _browser.WebDriver = _browser.FirefoxDebug();

        var user = new SteamUser();
        var doc = _browser.GetPageDocument(newUrl, 1000).Result;
        var main = Helper.FindNodesByDocument(doc, "div", "class", "responsive_page_template_content").Result.FirstOrDefault();
        if (main is not null)
        {
            // ID
            user.Id = ID;

            // User Url
            user.ProfilUrl = url;

            // Username span class actual_persona_name innertext
            var username = Helper.FindNodesByNode(main, "span", "class", "actual_persona_name").Result.FirstOrDefault();
            if (username is not null)
                user.Username = username.InnerText;
            // ProfileText div class profile_summary noexpand innertext
            var profileText = Helper.FindNodesByNode(main, "div", "class", "profile_summary noexpand").Result.FirstOrDefault();
            if (profileText is not null)
                user.ProfilText = profileText.InnerText.Trim();

            // AvatarUrl div class playerAvatarAutoSizeInner innerhtml split "
            var avatar = Helper.FindNodesByNode(main, "div", "class", "playerAvatarAutoSizeInner").Result.FirstOrDefault();
            if (avatar is not null)
            {
                var s = avatar.InnerHtml.Split('"'); // abfragen ob split[x] == 
                user.AvatarUrl = s.Where(x => x.Contains("https://avatars.cloudflare.steamstatic.com/")).FirstOrDefault();
            }

            // Level span class friendPlayerLevelNum innertext
            var level = Helper.FindNodesByNode(main, "span", "class", "friendPlayerLevelNum").Result.FirstOrDefault();
            if (level is not null)
                user.Level = int.Parse(level.InnerText);

            // BadgeIconUrl img class badge_icon small outerhtml split "
            var badge = Helper.FindNodesByNode(main, "img", "class", "badge_icon small").Result.FirstOrDefault();
            if (badge is not null)
            {
                var s = badge.OuterHtml.Split('"');
                user.BadgeIconUrl = s[1];
            }

            // Searching for div class showcase_content_bg showcase_stats_row, found 2?
            var showCaseContent = Helper.FindNodesByNode(main, "div", "class", "showcase_content_bg showcase_stats_row").Result;

            var nodes = new List<HtmlNode>();

            foreach (var shc in showCaseContent)
            {
                var node1 = Helper.FindNodesByNode(shc, "div", "class", "showcase_stat").Result;
                var node2 = Helper.FindNodesByNode(shc, "a", "class", "showcase_stat").Result;

                nodes.AddRange(node1);
                nodes.AddRange(node2);
            }

            foreach (var n in nodes)
            {
                if (n.InnerHtml.Contains("Errungenschaften"))
                {
                    var value = Helper.FindNodesByNode(n, "div", "class", "value").Result.FirstOrDefault();
                    int x = int.Parse(value.InnerText.Replace(",", ""));
                    user.Trophys = x;
                }
                else if (n.InnerHtml.Contains("Perfekte Spiele"))
                {
                    var value = Helper.FindNodesByNode(n, "div", "class", "value").Result.FirstOrDefault();
                    int x = int.Parse(value.InnerText.Replace(",", ""));
                    user.Platin = x;
                }
                else if (n.InnerHtml.Contains("Rezensionen"))
                {
                    var value = Helper.FindNodesByNode(n, "div", "class", "value").Result.FirstOrDefault();
                    int x = int.Parse(value.InnerText.Replace(",", ""));
                    user.Reviews = x;
                }
                else if (n.InnerHtml.Contains("Auf Wunschliste"))
                {
                    var value = Helper.FindNodesByNode(n, "div", "class", "value").Result.FirstOrDefault();
                    int x = int.Parse(value.InnerText.Replace(",", ""));
                    user.GamesOnWishlist = x;
                }
            }

            //--------------------------------------------------------------------------------------------------------------

            //// Block for this 2 div class showcase_content_bg showcase_stats_row - 0
            //var gameCollector = Helper.FindNodesByNode(showCaseContent[0],"div","class", "value").Result;

            //// Reviews div class showcase_stat 2
            //user.Reviews = int.Parse(gameCollector[2].InnerText); // knallt weil 62%

            //// GamesOnWishlist div class showcase_stat 3
            //user.GamesOnWishlist = int.Parse(gameCollector[3].InnerText);


            //// div class showcase_content_bg showcase_stats_row - 1
            //var rareTrophys = Helper.FindNodesByNode(showCaseContent[1],"div","class", "value").Result;

            //// Trophys div class showcase_stat 0 replace , 
            //user.Trophys = int.Parse(rareTrophys[0].InnerText.Replace(",", ""));

            //// Platin div class showcase_stat 1
            //user.Platin = int.Parse(rareTrophys[1].InnerText);




            // Games
            doc = _browser.GetPageDocument($"https://steamcommunity.com/profiles/{ID}/games/?tab=all", 1000).Result;
            var gameurls = new List<string>();
            var list = Helper.FindNodesByDocument(doc, "div", "id", "links_dropdown").Result;
            foreach (var l in list)
            {
                var shop = Helper.FindNodesByNode(l, "a", "class", "popup_menu_item2 tight").Result.FirstOrDefault();
                if (shop is not null)
                {
                    var s = shop.OuterHtml.Split('"');
                    var u = s.Where(x => x.Contains("https://store.steampowered.com/app/")).FirstOrDefault();
                    gameurls.Add(u);
                    Console.WriteLine();
                }
            }
            user.Games = gameurls.Distinct().ToArray();

            return user;
        }
        return null;
    }

    public long TranslateSteamID(string steamID)
    {
        long result = 0;

        var template = new Regex(@"STEAM_(\d):([0-1]):(\d+)");
        var matches = template.Matches(steamID);
        if (matches.Count <= 0) return 0;
        var parts = matches[0].Groups;
        if (parts.Count != 4) return 0;

        long x = long.Parse(parts[1].Value) << 24;
        long y = long.Parse(parts[2].Value);
        long z = long.Parse(parts[3].Value) << 1;

        result = 1 + (1 << 20) + x << 32 | y + z;
        return result;
    }

}

