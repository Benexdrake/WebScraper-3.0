using Newtonsoft.Json;
using Webscraper_API.Scraper.Dota2.Models;

namespace Webscraper_API.Scraper.Dota2.Controllers;

public class Dota_Api : IDota_Api
{
    private readonly Browser _browser;
    public Dota_Api(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }



    public async Task<int[]> GetAllIds()
    {
        List<int> ids = new List<int>();

        string url = "https://www.dota2.com/datafeed/herolist?language=german";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpResponseMessage response = client.GetAsync(
                        url).Result)
            {
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject< Rootobject >(responseBody);

                foreach (var item in result.result.data.heroes)
                {
                    ids.Add(item.id);
                }
            }
        }
        return ids.ToArray();
    }

    public async Task<Hero> GetHero(int id)
    {
        string url = "https://www.dota2.com/datafeed/herodata?language=german&hero_id=" + id;

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpResponseMessage response = client.GetAsync(
                        url).Result)
            {
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject< Rootobject >(responseBody).result;

                //result.data.heroes[0].imageUrl = $"https://cdn.cloudflare.steamstatic.com/apps/dota2/videos/dota_react/heroes/renders/{result.data.heroes[0].name_loc.ToLower().Replace(" ","_")}.png";
                //result.data.heroes[0].videoUrl = $"https://cdn.cloudflare.steamstatic.com/apps/dota2/videos/dota_react/heroes/renders/{result.data.heroes[0].name_loc.ToLower().Replace(" ", "_")}.webm";

                return Convert(result.data.heroes.FirstOrDefault()).Result;
            }
        }
    }

    private async Task<Hero> Convert(Hero h)
    {
        var nh = new Hero();



        return nh;
    }
}