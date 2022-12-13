namespace EF_Core_Console.Controller;

public class IMDbController : IIMDbController
{
    private readonly Browser _browser;
    private readonly IIMDb_API _iMDb_API;
    private readonly ImdbDBContext _context;

    public IMDbController(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
        _iMDb_API = service.GetRequiredService<IIMDb_API>();
        _context = service.GetRequiredService<ImdbDBContext>();
    }

    private void SaveMovie(Movie movie)
    {
        try
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
        }
    }

    public async Task LoadTop250()
    {
        var urls = _iMDb_API.GetMovieTop250Urls(_browser.GetPageDocument("https://www.imdb.com/chart/top/?ref_=nv_mv_250", 1000).Result).Result;
        foreach (var url in urls)
        {
            GetMovie(url);
        }
    }

    public async Task GetMovie(string url)
    {
        var split = url.Split('/');

        var m = _context.Movies.Where(x => x.Id.Equals(split[4])).FirstOrDefault();
        if (m is null)
        {
            var doc = _browser.GetPageDocument(url, 1000).Result;
            var movie = _iMDb_API.GetMovieByUrlAsync(url, doc).Result;
            SaveMovie(movie);
        }
    }
}
