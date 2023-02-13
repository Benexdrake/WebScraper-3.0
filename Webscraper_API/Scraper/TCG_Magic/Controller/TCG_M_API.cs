using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Webscraper_API.Scraper.TCG_Magic.Model;
using Webscraper_API.Scraper.TCG_Magic.Model.DbModel;

namespace Webscraper_API.Scraper.TCG_Magic.Controller
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

        public async Task<Card> Convert(Webscraper_API.Scraper.TCG_Magic.Model.JsonModel.Card c)
        {
            var multiverseIds = new List<MultiverseID>();
            var colors = new List<Color>();
            var colorIdentities = new List<ColorIdentity>();
            var keywords = new List<Keyword>();
            var finishes = new List<Finish>();
            var artistids = new List<ArtistId>();
            var games = new List<Game>();

            foreach (var item in c.Multiverse_ids)
            {
                multiverseIds.Add(new() { Id = item });
            }
            foreach (var item in c.Colors)
            {
                colors.Add(new() { T = item });
            }
            foreach (var item in c.Color_identity)
            {
                colorIdentities.Add(new() { T = item });
            }
            foreach (var item in c.Keywords)
            {
                keywords.Add(new() { T = item });
            }
            foreach (var item in c.Finishes)
            {
                finishes.Add(new() { T = item });
            }
            foreach (var item in c.Artist_ids)
            {
                artistids.Add(new() { T = item });
            }
            foreach(var item in c.Games)
            {
                games.Add(new() { T = item });
            }


            var nc = new Card();
            nc.Id= c.Id;
            nc.Name= c.Name;
            nc.Oracle_id = c.Oracle_id;
            nc.Tcgplayer_id= c.Tcgplayer_id;
            nc.Cardmarket_id = c.Cardmarket_id;
            nc.Name = c.Name;
            nc.Lang = c.Lang;
            nc.Released_at = c.Released_at;
            nc.Uri = c.Uri;
            nc.Scryfall_uri = c.Scryfall_uri;
            nc.Layout= c.Layout;
            nc.Highres_image= c.Highres_image;
            nc.Image_status = c.Image_status;
            nc.Mana_cost = c.Mana_cost;
            nc.Cmc = c.Cmc;
            nc.Type_line= c.Type_line;
            nc.Oracle_text= c.Oracle_text;
            
            nc.Reserved = c.Reserved;
            nc.Foil= c.Foil;
            nc.Nonfoil= c.Nonfoil;
            nc.Oversized = c.Oversized;
            nc.Promo= c.Promo;
            nc.Reprint = c.Reprint;
            nc.Variation = c.Variation;
            nc.Set_id = c.Set_id;
            nc.Set = c.Set;
            nc.Set_name = c.Set_name;
            nc.Set_type= c.Set_type;
            nc.Set_uri = c.Set_uri;
            nc.Set_search_uri= c.Set_search_uri;
            nc.Rulings_uri = c.Rulings_uri;
            nc.Prints_search_uri = c.Prints_search_uri;
            nc.Collector_number= c.Collector_number;
            nc.Digital = c.Digital;
            nc.Card_back_id= c.Card_back_id;
            nc.Artist = c.Artist;
            nc.Illustration_id= c.Illustration_id;
            nc.Border_color = c.Border_color;
            nc.Frame = c.Frame;
            nc.Full_art = c.Full_art;
            nc.Textless = c.Textless;
            nc.Booster = c.Booster;
            nc.Story_spotlight= c.Story_spotlight;
            nc.Edhrec_rank = c.Edhrec_rank;
            nc.Penny_rank = c.Penny_rank;

            nc.Multiverse_ids = multiverseIds.ToArray();
            nc.Colors = colors.ToArray();
            nc.Color_identity = colorIdentities.ToArray();
            nc.Keywords = keywords.ToArray();
            nc.Finishes = finishes.ToArray();
            nc.Artist_ids = artistids.ToArray();
            nc.Games = games.ToArray();

            nc.Image_uri = new()
            {
                Art_crop = c.Image_uris.Art_crop,
                Border_crop = c.Image_uris.Border_crop,
                Large = c.Image_uris.Large,
                Small = c.Image_uris.Small,
                Normal = c.Image_uris.Normal,
                Png = c.Image_uris.Png
            };
            nc.Legality = new()
            {
                Alchemy = c.Legalities.Alchemy,
                Standard = c.Legalities.Standard,
                Brawl = c.Legalities.Brawl,
                Commander = c.Legalities.Commander,
                Duel = c.Legalities.Duel,
                Explorer = c.Legalities.Explorer,
                Future = c.Legalities.Future,
                Gladiator = c.Legalities.Gladiator,
                Historic = c.Legalities.Historic,
                Historicbrawl = c.Legalities.Historic,
                Legacy = c.Legalities.Legacy,
                Modern = c.Legalities.Modern,
                Oldschool = c.Legalities.Oldschool,
                Pauper = c.Legalities.Pauper,
                Paupercommander = c.Legalities.Paupercommander,
                Penny = c.Legalities.Penny,
                Pioneer = c.Legalities.Pioneer,
                Premodern = c.Legalities.Premodern,
                Vintage = c.Legalities.Vintage
            };
            nc.Price = new()
            {
                Eur = c.Prices.Eur,
                Eur_foil = c.Prices.Eur_foil,
                Tix = c.Prices.Tix,
                Usd = c.Prices.Usd,
                Usd_etched = c.Prices.Usd_etched,
                Usd_foil = c.Prices.Usd_foil
            };
            nc.Related_uri = new()
            {
                Edhrec = c.Related_uris.Edhrec,
                Gatherer = c.Related_uris.Gatherer,
                Tcgplayer_infinite_articles = c.Related_uris.Tcgplayer_infinite_articles,
                Tcgplayer_infinite_decks = c.Related_uris.Tcgplayer_infinite_decks
            };
            nc.Purchase_uri = new()
            {
                Cardhoarder = c.Purchase_uris.Cardhoarder,
                Cardmarket = c.Purchase_uris.Cardmarket,
                Tcgplayer = c.Purchase_uris.Tcgplayer
            };
            return nc;
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
