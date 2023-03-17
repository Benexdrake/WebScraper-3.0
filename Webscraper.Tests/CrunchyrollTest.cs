namespace Webscraper.Tests;

public class CrunchyrollTest
{
    private readonly ICR_API _api;

    public CrunchyrollTest(ICR_API api)
    {
        _api = api;
    }

    [Fact]
    public async Task GetAnimeByUrl()
    {
        var anime = await _api.GetAnimeByUrlAsync("https://www.crunchyroll.com/de/series/GYEX24PV6/digimon-adventure-2020", 3000);
        Assert.NotNull(anime);
    }
}
