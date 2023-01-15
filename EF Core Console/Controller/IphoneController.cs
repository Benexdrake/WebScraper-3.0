using Webscraper_API.Scraper.Apple.Iphone.Controllers;
using Webscraper_API.Scraper.Apple.Iphone.Models;

namespace EF_Core_Console.Controller;

internal class IphoneController
{
    private readonly Browser _browser;
    private readonly I_API _api;

    public IphoneController(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
        _api = service.GetRequiredService<I_API>();
    }

    public async Task GetAllIphones()
    {
        string u = "https://www.gsmarena.com/results.php3?sQuickSearch=yes&sName=iphone%20apple";

        var urls = _api.GetPhoneUrlsAsync(u).Result;

        List<IPhone> iphones= new List<IPhone>();

        foreach (var url in urls)
        {
            var iphone = _api.GetPhoneAsync(url).Result;
            if(iphone is not null)
                iphones.Add(iphone);
        }

        var json = JsonConvert.SerializeObject(iphones,Formatting.Indented);
        await File.WriteAllTextAsync("Iphones.json", json);

    }
}
