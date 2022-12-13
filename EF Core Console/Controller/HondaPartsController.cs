public class HondaPartsController : IHondaPartsController
{
    private readonly Browser _browser;
    private readonly IHonda_Api _api;
    private readonly HondaPartsDbContext _context;

    public HondaPartsController(IServiceProvider service, IConfiguration conf)
    {
        _browser = service.GetRequiredService<Browser>();
        _api = service.GetRequiredService<IHonda_Api>();
        _context = service.GetRequiredService<HondaPartsDbContext>();

    }

    public async Task FullUpdate(bool relatedparts, string url)
    {
        var urls = await GetAllCategorieWithPartsUrls(url);
        if (relatedparts)
            await GetAllPartUrls(urls);
        await GetAllParts(urls.ToArray());
    }

    private async Task<List<CategoryUrl>> GetAllCategorieWithPartsUrls(string mainUrl)
    {
        var doc = await _browser.GetPageDocument(mainUrl, 0);

        var mainUrls = await _api.GetMainCategoryUrls(doc);

        List<CategoryUrl> urls = new();
        List<CategoryUrl> partsUrls = new();

        foreach (var url in mainUrls)
        {
            doc = await _browser.GetPageDocument(url, 0);
            var categoryUrls = await _api.GetCategoriesUrls(doc);
            if (categoryUrls != null)
                urls.AddRange(categoryUrls);
        }
        int i = 1;
        foreach (var url in urls)
        {
            doc = await _browser.GetPageDocument(url.Url, 0);
            var partsurl = await _api.GetPartsUrls(doc);
            if (partsurl is not null)
            {
                foreach (var item in partsurl)
                {
                    partsUrls.Add(new CategoryUrl()
                    {
                        Url = item,
                        SubCategory = url.SubCategory,
                        Category = url.Category
                    });
                }
            }
            Log.Logger.Information($"{i}/{urls.Count} - Urls: {partsurl.Length}");
            i++;
        }
        return partsUrls;
    }

    private async Task<List<CategoryUrl>> GetAllPartUrls(List<CategoryUrl> urls)
    {
        List<string> relPartsUrls = new();
        int i = 0;
        foreach (var url in urls)
        {
            relPartsUrls.Add(url.Url);
            var doc = await _browser.GetPageDocument(url.Url, 0);

            var relParts = _api.GetRelatedPartsUrls(doc).Result;
            relPartsUrls.AddRange(relParts);

            i++;
            Log.Logger.Information(EF_Core_Console.Helper.Percent(i, urls.Count) + " / 100%");
        }
        var urlsDistinct = relPartsUrls.Distinct().ToList();

        foreach (var url in urlsDistinct)
        {
            urls.Add(new CategoryUrl()
            {
                Category = "-",
                SubCategory = "-",
                Url = url
            });
        }
        return urls;
    }

    private async Task GetAllParts(CategoryUrl[] urls)
    {
        int i = 0;
        foreach (var url in urls)
        {
            GetPart(url);
            i++;
            Log.Logger.Information(EF_Core_Console.Helper.Percent(i, urls.Length) + " / 100%");
        }
    }

    public async Task GetPart(CategoryUrl url)
    {
        var p = _context.HondaParts.Where(x => x.Url.Equals(url.Url)).FirstOrDefault();
        if (p is null)
        {
            var doc = await _browser.GetPageDocument(url.Url, 0);
            var part = _api.GetPart(doc, url).Result;
            SavePart(part);
        }
    }

    private void SavePart(NewParts part)
    {
        try
        {
            _context.HondaParts.Add(part);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
        }
        
    }

}
