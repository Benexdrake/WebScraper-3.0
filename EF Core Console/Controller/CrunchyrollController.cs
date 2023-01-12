using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        _context.Animes.Add(anime);
        _context.SaveChanges();   
    }


    private void SaveEpisodes(Episode episode)
    {
        _context.Episodes.Add(episode);
        _context.SaveChanges();
    }

    private void UpdateAnime(Anime anime, Anime aDb)
    {
        //aDb.Episodes = anime.Episodes;
        _context.SaveChanges();
    }

    public async Task FullUpdateAnimes()
    {
        var urls = _api.GetAllAnimeUrlsAsync().Result;
        try
        {
            int i = 0;
            foreach (var url in urls)
            {
                i++;
                var a = _context.Animes.Where(a => a.Url.Equals(url)).FirstOrDefault();
                if (a is null)
                {
                    var AE = _api.GetAnimewithEpisodes(url, 2000).Result;
                    if (AE is not null)
                    {
                        SaveAnime(AE.Anime);
                        foreach (var e in AE.Episodes)
                        {
                            var episode = _context.Episodes.Where(a => a.Id.Equals(e.Id)).FirstOrDefault();
                            if(episode is null)
                            {
                                SaveEpisodes(e);
                            }
                        }
                    }
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

    public async Task SimulcastUpdate()
    {
        //var urls = _api.GetSimulcastUpdateUrlsAsync().Result;

        var urls = _api.GetDailyUpdateAsync().Result;
        //var animes = _context.Animes.ToList();

        var updateList = new List<Anime>();

        try
        {
            int i = 0;
            foreach (var url in urls)
            {
                i++;
                var anime = _api.GetAnimeByUrlAsync(url, 2000).Result;
                
                var percent = Helper.Percent(i, urls.Length);
                var a = _context.Animes.Where(a => a.Url.Equals(url)).FirstOrDefault();
                if (anime is not null)
                {
                    if (a is null)
                    {
                        SaveAnime(anime);
                        Log.Logger.Information($">>> Insert: Animes found: {percent}%/100%");
                        Log.Logger.Information(url);
                    }
                    else
                    {
                        UpdateAnime(anime, a);
                        Log.Logger.Information($">>> Update: Animes found: {percent}%/100%");
                        Log.Logger.Information(url);
                    }
                }
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

    public async Task Debug(string url)
    {
        //var anime = _api.GetAnimeByUrlAsync(url, 2000).Result;
        //var episodes = _api.GetEpisodes(url, 2000).Result;
        //Console.WriteLine(anime.Name);
        //SaveAnime(anime);

        //var animeEpisode = _api.GetAnimewithEpisodes(url, 2000).Result;
        //Console.WriteLine();
        //SaveAnime(animeEpisode.Anime);

        var episode = _context.Episodes.FirstOrDefault();


        var e = _api.GetEpisodeDetails(episode).Result;

        //SaveEpisodes(animeEpisode.Episodes);


        Console.WriteLine();
    }
}
