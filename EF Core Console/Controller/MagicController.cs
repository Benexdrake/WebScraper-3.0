using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webscraper_API.Scraper.TCG_Magic.Controller;
using Webscraper_API.Scraper.TCG_Magic.Model;
using Webscraper_API.Scraper.TCG_Magic.Old;

namespace EF_Core_Console.Controller
{
    public class MagicController
    {
        private readonly TCG_M_API _api;
        private readonly Browser _browser;
        public MagicController(IServiceProvider service)
        {
            _api = service.GetRequiredService<TCG_M_API>();
            _browser= service.GetRequiredService<Browser>();
        }

        public async Task Test()
        {
            var seturls = await _api.GetAllSetUrls();
            List<Card> cards = new List<Card>();
            List<CardUrl> urls = new();
            int i = 1;
            foreach (var set in seturls)
            {
                var s = await _api.GetSet(set);


                var cardUrls = await _api.GetAllCardBySetUrl(set);
                urls.AddRange(cardUrls);

                Console.WriteLine($"{Helper.Percent(i,seturls.Length)}% / 100%");
                
                i++;
            }
            i = 1;
            foreach (var u in urls)
            {
                Console.WriteLine($"{Helper.Percent(i, urls.Count)}% / 100%");
                var card = _api.GetCard($"https://api.scryfall.com/cards/{u.Id}?format=json&pretty=true").Result;
                cards.Add(card);
                i++;
            }


            var json = JsonConvert.SerializeObject(cards, Formatting.Indented);
            File.WriteAllText("MagicCards.json", json);
            Console.WriteLine("Saved MagicCards.Json");
        }
    }
}
