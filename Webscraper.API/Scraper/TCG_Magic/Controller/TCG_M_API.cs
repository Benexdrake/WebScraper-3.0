using Newtonsoft.Json;
using Webscraper.API.Interfaces;

namespace Webscraper.API.Scraper.TCG_Magic.Controller
{
    public class TCG_M_API : ITCG_M_API
    {
        private readonly IServiceProvider _service;
        private readonly Browser _browser;

        public TCG_M_API(IServiceProvider service)
        {
            _service = service;
            _browser = service.GetRequiredService<Browser>();
        }
        public async Task<string[]> GetAllSetUrls()
        {
            List<string> urls = new List<string>();

            string url = "https://scryfall.com/sets";

            var doc = _browser.GetPageDocument(url, 1000).Result;

            var main = FindNodesByDocument(doc, "div", "class", "checklist-wrapper").Result.FirstOrDefault();
            var tbody = FindNodesByNode(main, "tr").Result;
            for (int i = 1; i < tbody.Count; i++)
            {
                var set = FindNodesByNode(tbody[i], "a", "href").Result.FirstOrDefault();
                if (set != null)
                {
                    if (!set.OuterHtml.Split('"')[1].Contains("https://scryfall.com"))
                    {
                        urls.Add("https://scryfall.com" + set.OuterHtml.Split('"')[1]);
                    }
                    else
                    {
                        urls.Add(set.OuterHtml.Split('"')[1]);
                    }
                }
            }
            return urls.ToArray();
        }

        public async Task<CardUrl[]> GetAllCardBySetUrl(string setUrl)
        {
            List<CardUrl> cards = new();

            var doc = _browser.GetPageDocument(setUrl, 1000).Result;

            var main = FindNodesByDocument(doc, "div", "class", "main").Result.FirstOrDefault();
            if (main is null)
            {
                return null;
            }
            var cardsItemList = FindNodesByNode(main, "div", "class", "card-grid-item").Result;
            foreach (var cardItem in cardsItemList)
            {
                var card = FindNodesByNode(cardItem, "a", "class", "card-grid-item-card").Result.FirstOrDefault();
                if (card is not null)
                {
                    var split = card.OuterHtml.Split('"');
                    cards.Add(new CardUrl()
                    {
                        Url = split[3],
                        Id = card.ParentNode.OuterHtml.Split('"')[3]
                    });
                }
            }
            return cards.ToArray();
        }

        public async Task<Card> GetCard(string id)
        {
            string url = $"https://api.scryfall.com/cards/{id}?format=json&pretty=true";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    using (HttpResponseMessage response = client.GetAsync(
                                url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        var card = JsonConvert.DeserializeObject<Card>(responseBody);

                        if (card is not null)
                        {
                            return card;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return await GetCard(url);
            }
            return null;
        }


        #region Private Nodes

        private async Task<List<HtmlNode>> FindNodesByNode(HtmlNode node, string a = "", string b = "", string c = "")
        {
            return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }

        private async Task<List<HtmlNode>> FindNodesByDocument(HtmlDocument document, string a = "", string b = "", string c = "")
        {
            return document.DocumentNode.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }

        #endregion
    }
}
