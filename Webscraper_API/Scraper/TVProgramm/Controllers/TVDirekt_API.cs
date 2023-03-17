namespace Webscraper.API.Scraper.TVProgramm.Controllers;

public class TVDirekt_API
{
    private readonly Browser _browser;
    public TVDirekt_API(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }

    public async Task<TV[]> GetAllTVProgramms(string date)
    {
        string url = "https://m.tvdirekt.de/component/pitbroadcast/?date=1678147200&timeGroup=1&limitstart=0&stationType=1&filter%5Bgenre%5D=2&ajax=0";

        //var span = TimeSpan.Parse("1678147200");
        var dt = DateTime.Now;
        var doc = _browser.GetPageDocument(url, 1000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "id", "siteMain").Result.FirstOrDefault();


        return null;
    }

}
