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
        var urls = _iMDb_API.GetMovieTop250Urls().Result;
        int i = 1;
        foreach (var url in urls)
        {
            await GetMovie(url);
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>> " + Helper.Percent(i,urls.Count) + "% / 100%");
            i++;
        }
    }

    public async Task<Movie> GetMovie(string url)
    {
        var split = url.Split('/');

        var m = _context.Movies.Where(x => x.Id.Equals(split[4])).FirstOrDefault();
        if (m is null)
        {
            var movie = _iMDb_API.GetMovieByUrlAsync(url).Result;
            SaveMovie(movie);
            return movie;
        }
        return null;
    }

    public async Task GetFavorits(string id)
    {
        var urls = _iMDb_API.GetFavoritUrlsAsync(id).Result;

        for (int i = 0; i < urls.Length; i++)
        {
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>> " + Helper.Percent(i, urls.Length) + "% / 100%");
            var movie = GetMovie(urls[i]).Result;
            if(movie is not null)
                Console.WriteLine($"Found: {movie.Title}");
        }
    }
}
