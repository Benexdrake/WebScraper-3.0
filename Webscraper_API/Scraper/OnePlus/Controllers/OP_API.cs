namespace Webscraper_API.Scraper.OnePlus.Controllers;

public class OP_API : IOP_API
{
    private readonly Browser _browser;
    public OP_API(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }
    public async Task<string[]> GetPhoneUrls()
    {
        string url = "https://www.oneplus.com/at/store/phone";

        var doc = _browser.GetPageDocument(url, 1000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "products-content").Result.FirstOrDefault();
        if (main is not null)
        {
            var urls = new List<string>();
            var list = Helper.FindNodesByNode(main, "div", "class", "main-products-one").Result;

            foreach (var item in list)
            {
                var split = item.InnerHtml.Split('"');
                urls.Add(split[1]);
            }
            return urls.ToArray();

        }
        return null;
    }

    public async Task GetPhoneByUrl(string url, int time)
    {
        var doc = _browser.GetPageDocument(url, time).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "section-choose-phone flex").Result.FirstOrDefault();
        if (main is not null)
        {
            var list = Helper.FindNodesByNode(main, "div", "class", "one-collapse-item").Result;
            foreach (var item in list)
            {

            }
        }
        Console.WriteLine();
    }
}
