namespace EF_Core_Console.Controller;
public class CrunchyrollController : ICrunchyrollController
{
    private readonly Browser _browser;
    private readonly ICR_API _api;
    private readonly CrunchyrollDBContext _context;

    public CrunchyrollController(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
        _api = service.GetRequiredService<ICR_API>();
        _context = service.GetRequiredService<CrunchyrollDBContext>();
    }

    private void SaveAnime(Anime anime)
    {
        try
        {
            _context.Animes.Add(anime);
            _context.SaveChanges();
        }
        catch (Exception e)
        {

            Log.Logger.Error(e.Message);
        }
    }

    public async Task FullUpdateAnimes()
    {
        var urls = GetUrls().Result;
        try
        {
            int i = 0;
            foreach (var url in urls)
            {
                i++;
                var a = _context.Animes.Where(a => a.Url.Equals(url)).FirstOrDefault();
                if (a is null)
                {
                    var doc = _browser.GetPageDocument(url, 2000).Result;
                    var anime = _api.GetAnimeByUrlAsync(url, doc).Result;
                    if (anime is not null)
                        SaveAnime(anime);
                }
                var percent = Helper.Percent(i, urls.Length);
                Log.Logger.Information($"Animes found: {percent}%/100%");
                Log.Logger.Information(url);
            }
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
        }
        finally
        {
            _browser.CloseDriver();
        }
    }

    private async Task<string[]> GetUrls()
    {
        return _api.GetAnimeUrlList(_browser.WebDriver).Result;
    }

    public async Task Debug(string url)
    {
        _browser.WebDriver = _browser.FirefoxDebug();
        var doc = _browser.GetPageDocument(url, 2000).Result;
        var anime = _api.GetAnimeByUrlAsync(url, doc).Result;
        Console.WriteLine(anime.Name);
        Console.WriteLine();
    }
}
