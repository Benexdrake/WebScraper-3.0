using OpenQA.Selenium;
using Webscraper.API.Interfaces;

namespace Webscraper.API.Scraper.Twitch.Controllers;

public class Twitch_API : ITwitch_API
{
    private readonly Browser _browser;
    public Twitch_API(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }

    public async Task<TwitchUser> GetTwitchProfil(string url)
    {
        var split = url.Split("/");

        var doc = _browser.GetPageDocument($"https://www.twitch.tv/{split[3]}/about", 1000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "Layout-sc-1xcs6mc-0 bSoSIm").Result.FirstOrDefault();
        if (main is not null)
        {
            var user = new TwitchUser();
            List<TwitchGame> gameList = new();

            // Username
            var username = Helper.FindNodesByDocument(doc, "div", "class", "Layout-sc-1xcs6mc-0 dVelak").Result.FirstOrDefault();

            // AvatarUrl

            var avatarUrl = Helper.FindNodesByDocument(doc, "img", "class", "InjectLayout-sc-1i43xsx-0 bEwPpb tw-image tw-image-avatar").Result;
            var avatarSplit = avatarUrl.LastOrDefault().OuterHtml.Split('"');

            // BannerUrl xxxxxxxxxxxxxxxxx
            var bannerUrl = Helper.FindNodesByDocument(doc, "div", "class", "channel-root__player channel-root__player--offline").Result.FirstOrDefault();

            string banner = string.Empty;

            if (bannerUrl is not null)
            {
                var background = Helper.FindNodesByNode(bannerUrl, "div", "style", "background-image").Result.FirstOrDefault();
                var bs = background.OuterHtml.Split("/");
                banner = $"https://{bs[2]}/{bs[3]}/{bs[4].Split(")")[0]}";
            }
            // Follower Count
            var follower = Helper.FindNodesByDocument(doc, "p", "class", "CoreText-sc-1txzju1-0 eUotgq").Result.FirstOrDefault();

            // User Information xxxxxxxxxxxxx
            var information = Helper.FindNodesByDocument(doc, "p", "class", "CoreText-sc-1txzju1-0 kLFSJC").Result.FirstOrDefault();

            // Description div class panel-description list
            var descriptionBlock = Helper.FindNodesByDocument(doc, "div", "class", "Layout-sc-1xcs6mc-0 ScTypeset-sc-i73f0c-0 dRZPzM flfwOo tw-typeset").Result.FirstOrDefault();
            string d = string.Empty;
            if (descriptionBlock is not null)
            {

                var desc = Helper.FindNodesByNode(descriptionBlock, "p", "", "").Result;
                for (int i = 0; i < desc.Count; i++)
                {
                    if (i < desc.Count - 1)
                        d += desc[i].InnerText + "\n";
                    else
                        d += desc[i].InnerText;

                }
            }

            // search for button and click, its faster as reload the Site
            var button = _browser.WebDriver.FindElements(By.ClassName("ScTextWrapper-sc-iekec1-1")).FirstOrDefault();
            if (button is not null)
            {
                button.Click();
                await Task.Delay(1000);

                string page = _browser.WebDriver.PageSource;
                doc = new HtmlDocument();
                doc.LoadHtml(page);
                // Last streamed Games
                var block = Helper.FindNodesByDocument(doc, "div", "class", "ScTower-sc-1sjzzes-0 jOMJnS tw-tower").Result.FirstOrDefault();
                if (block is not null)
                {
                    var games = Helper.FindNodesByNode(block, "div", "class", "InjectLayout-sc-1i43xsx-0 eptOJT").Result;

                    foreach (var g in games)
                    {
                        var game = new TwitchGame();
                        var id = Helper.FindNodesByNode(g, "a", "class", "ScCoreLink-sc-16kq0mq-0 jSrrlW game-card__link tw-link").Result.FirstOrDefault();
                        var idsplit = id.OuterHtml.Split('"')[7].Split("=");
                        var image = Helper.FindNodesByNode(g, "img", "class", "tw-image").Result.FirstOrDefault();
                        var imageSplit = image.OuterHtml.Split('"');
                        var name = Helper.FindNodesByNode(g, "h2", "class", "CoreText-sc-1txzju1-0 eYbmNi").Result.FirstOrDefault();

                        game.Id = idsplit[2];
                        game.Name = name.InnerText;
                        game.ImageUrl = imageSplit[5];
                        game.Url = $"https://www.twitch.tv/directory/game/{name.InnerText}";

                        gameList.Add(game);
                    }
                }
            }
            user.Id = split[3];
            user.Name = username.InnerText;
            user.ProfilUrl = $"https://www.twitch.tv/{split[3]}";
            user.AvatarUrl = avatarSplit[5];
            user.BannerUrl = banner;
            user.Follower = follower.InnerText.Replace(" Follower", "");
            user.Information = information.InnerText;
            user.Description = d;
            user.LastStreamedGames = gameList.ToArray();

            return user;
        }
        return null;
    }
}
