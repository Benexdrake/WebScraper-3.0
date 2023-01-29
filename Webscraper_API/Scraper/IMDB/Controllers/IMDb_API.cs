using System;

namespace Webscraper_API.Scraper.IMDB.Controllers
{
    public class IMDb_API : IIMDb_API
    {
        private readonly Browser _browser;
        public IMDb_API(IServiceProvider service)
        {
            _browser = service.GetRequiredService<Browser>();
        }

        #region Get a Movie by Url
        public async Task<Movie> GetMovieByUrlAsync(string url)
        {
            var doc = _browser.GetPageDocument(url, 1000).Result;

            BuildModels.Builder b = new BuildModels.Builder();

            var main = FindNodesByDocument(doc, "section", "class", "ipc-page-section").Result.ToList();

            if (main.Count == 0)
            {
                Console.WriteLine($"ID: {url.Split('/')[4]} übersprungen...");
                return null;
            }

            var sub = FindNodesByNode(main[0], "li", "class", "ipc-metadata-list__item").Result.ToList();
            var boxOffice = FindNodesByDocument(doc, "section", "data-testid", "BoxOffice").Result.FirstOrDefault();
            var handlung = FindNodesByDocument(doc, "section", "data-testid", "Storyline").Result.FirstOrDefault();
            var TechSpecs = FindNodesByDocument(doc, "section", "data-testid", "TechSpecs").Result.FirstOrDefault();
            var details = FindNodesByDocument(doc, "section", "data-testid", "Details").Result.FirstOrDefault();
            if (details == null)
            {
                return null;
            }
            var detail = FindNodesByNode(details, "li", "role", "presentation").Result;



            b.m
             .Id(GetId(url))
             .Title(GetTitle(main[0]))
             //.Trailer(GetTrailer(main[0]))
             .Rating(GetRating(main[0]))
             .Genres(GetGenre(main[0]))
             .Description(GetDescription(main[0]))
             .Url(url)
             .ImgUrl(GetImgUrl(main[0]))
             .Director(GetDirector(sub[0]))
             .Script(GetScript(sub[1]))
             .MainCast(GetMainCast(main[0]))
             .Budget(GetBudget(boxOffice))
             .Runtime(GetRuntime(TechSpecs));

            foreach (var d in detail)
            {
                string[] content =
                {
                    GetOriginCountry(d.InnerText),
                    GetLocation(d.InnerText),
                    GetKnownAs(d.InnerText),
                    GetProductionCompanies(d.InnerText),
                    GetProductionCompany(d.InnerText),
                    GetReleaseDate(d.InnerText),
                    GetOriginCountries(d.InnerText),
                };


                if (!string.IsNullOrWhiteSpace(content[0]))
                {
                    b.m.OriginCountry(content[0]);
                }
                if (!string.IsNullOrWhiteSpace(content[1]))
                {
                    b.m.Location(content[1]);
                }
                if (!string.IsNullOrWhiteSpace(content[2]))
                {
                    b.m.KnonAs(content[2]);
                }
                if (!string.IsNullOrWhiteSpace(content[3]))
                {
                    b.m.ProductionCompanies(content[3]);
                }
                if (!string.IsNullOrWhiteSpace(content[4]))
                {
                    b.m.ProductionCompanies(content[4]);
                }
                if (!string.IsNullOrWhiteSpace(content[5]))
                {
                    b.m.ReleaseDate(content[5]);
                }
                if (!string.IsNullOrWhiteSpace(content[6]))
                {
                    b.m.OriginCountry(content[6]);
                }
            }
            // Return Movie
            return await Task.FromResult(b.m.GetMovie());
        }
        #endregion

        #region Favorit List by List ID

        public async Task<string[]> GetFavoritUrlsAsync(string id)
        {
            int maxSides = 1;
            string url = $"https://www.imdb.com/list/{id}/?sort=list_order,asc&st_dt=&mode=simple&page=1&ref_=ttls_vw_smp";
            List<string> favUrls = new();
            var doc = _browser.GetPageDocument(url, 1000).Result;

            var counterNode = Helper.FindNodesByDocument(doc,"div","class", "desc lister-total-num-results").Result.FirstOrDefault();
            if(counterNode is not null)
            {
                var counter = int.Parse(counterNode.InnerText.Replace("titles", " ").Trim());
                if (counter % 100 == 0)
                    maxSides = counter / 100;
                else
                    maxSides = (counter / 100) + 1;

                for (int i = 1; i <= maxSides; i++)
                {
                    url = $"https://www.imdb.com/list/{id}/?sort=list_order,asc&st_dt=&mode=simple&page={i}&ref_=ttls_vw_smp";
                    var urls = GetUrlsFromSide(url).Result;
                    if(urls is not null)
                        favUrls.AddRange(urls);
                }
                return favUrls.ToArray();
            }
            return null;
        }

        private async Task<string[]> GetUrlsFromSide(string url)
        {
            var doc = _browser.GetPageDocument(url, 1000).Result;
            var main = Helper.FindNodesByDocument(doc, "div", "class", "lister-list").Result.FirstOrDefault();
            if(main is not null)
            {
                List<string> urls = new();
                var listNodes = Helper.FindNodesByNode(main, "div", "class", "lister-item mode-simple").Result;
                foreach (var list in listNodes)
                {
                    var u = "https://www.imdb.com" + list.InnerHtml.Split('"')[3];
                    urls.Add(u);
                }
                return urls.ToArray();
            }
            return null;
        }

        #endregion


        #region Get Urls from Top 250 Site
        public async Task<List<string>> GetMovieTop250Urls()
        {
            string url = "https://www.imdb.com/chart/top/";
            
            var doc = _browser.GetPageDocument(url, 1000).Result;
            List<string> movieUrls = new List<string>();


            var main = FindNodesByDocument(doc, "div", "id", "main").Result.FirstOrDefault();

            var list = FindNodesByNode(main, "td", "class", "titleColumn").Result.ToList();

            foreach (var l in list)
            {
                var u = l.InnerHtml;

                var split = u.Split('/');

                var newUrl = "https://www.imdb.com/title/" + split[2];
                movieUrls.Add(newUrl);
            }
            return await Task.FromResult(movieUrls);
        }
        #endregion
       
        #region Get Region
        private string GetId(string url)
        {
            var split = url.Split('/');
            string id = string.Empty;
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Contains("title"))
                {
                    id = split[i + 1];
                    break;
                }
            }

            return id;
        }

        private string GetTrailer(HtmlNode node)
        {
            var trailer = FindNodesByNode(node, "video", "class", "jw-video jw-reset").Result.FirstOrDefault();
            if (trailer != null)
            {
                // findet die Trailer URL
                var outer = trailer.OuterHtml.Split('"');

                var url = outer[13];


                return url;
            }
            return "";
        }

        private string GetImgUrl(HtmlNode node)
        {
            var pic = FindNodesByNode(node, "img", "class", "ipc-image").Result.FirstOrDefault();
            if (pic != null)
            {
                var split = pic.OuterHtml.Split('"');
                var image = split[7].Split("@")[0] + "@._V1_.jpg";
                return image;
            }
            return "";
        }

        private string GetTitle(HtmlNode node)
        {
            return FindNodesByNode(node, "h1", "data-testid", "hero-title-block__title").Result.FirstOrDefault().InnerText;
        }

        // Get Genre
        private string GetGenre(HtmlNode node)
        {
            var g = FindNodesByNode(node, "div", "data-testid", "genres").Result.FirstOrDefault();
            if (g == null)
                return "-";
            var genres = FindNodesByNode(node, "span", "class", "ipc-chip__text").Result;

            string result = string.Empty;

            foreach (var genre in genres)
            {
                result += genre.InnerText + ", ";
            }

            return result.Substring(0, result.Length - 2);
        }

        // Get Rating
        private double GetRating(HtmlNode node)
        {
            var r = FindNodesByNode(node, "div", "data-testid", "hero-rating-bar__aggregate-rating__score").Result.FirstOrDefault();
            if (r != null)
                return Double.Parse(r.InnerText.Split('/')[0]);
            return 0;
        }

        private string GetScript(HtmlNode node)
        {
            var script1 = FindNodesByNode(node, "a", "class", "ipc-metadata").Result.ToList();
            var script2 = FindNodesByNode(node, "span", "class", "ipc-metadata").Result.ToList();

            var script = string.Empty;

            if (script1.Count == 0 || script2.Count == 0)
                return "";

            if (script1.Count < script2.Count)
            {
                for (int i = 0; i < script1.Count; i++)
                {
                    script += $"{script1[i].InnerText} {script2[i + 1].InnerText} ";
                }
            }
            else if (script1.Count == script2.Count)
            {
                script += $"{script1[0].InnerText}";
            }
            else
            {
                for (int i = 0; i < script2.Count; i++)
                {
                    script += $"{script1[i + 1].InnerText} {script2[i].InnerText} ";
                }
            }

            return script;
        }

        private string GetMainCast(HtmlNode node)
        {
            var maincast = FindNodesByNode(node, "ul", "class", "ipc-inline-list ipc-inline-list--show-dividers ipc-inline-list--inline ipc-metadata-list-item__list-content baseAlt").Result.FirstOrDefault();
            if (maincast == null)
                return "";
            var maincastSubs = FindNodesByNode(maincast, "a", "class", "ipc-metadata-list-item__list-content-item ipc-metadata-list-item__list-content-item--link").Result;

            string cast = "";

            foreach (var mc in maincastSubs)
            {
                cast += $" {mc.InnerText},";
            }

            return cast.Substring(0, cast.Length - 2);
        }

        private string GetDirector(HtmlNode node)
        {
            var d = FindNodesByNode(node, "a", "class", "ipc-metadata-list-item__list-content-item ipc-metadata-list-item__list-content-item--link").Result.FirstOrDefault();

            if (d != null)
                return d.InnerText;

            return "";
        }

        private string GetDescription(HtmlNode node)
        {
            return FindNodesByNode(node, "span", "role", "presentation").Result[1].InnerText;
        }

        private string GetReleaseDate(string text)
        {
            if (text.Contains("Erscheinungsdatum"))
            {
                // Erscheinungsdatum
                return text.Replace("Erscheinungsdatum", "");
            }
            return "";
        }

        private string GetOriginCountry(string text)
        {
            if (text.Contains("Herkunftsland"))
            {
                // Herkunftsland
                return text.Replace("Herkunftsland", "");
            }
            return "";
        }
        private string GetOriginCountries(string text)
        {
            if (text.Contains("Herkunftsländer"))
            {
                // Herkunftsland
                return text.Replace("Herkunftsländer", "");
            }
            return "";
        }

        private string GetBudget(HtmlNode node)
        {
            if (node != null)
            {
                var bo = FindNodesByNode(node, "li", "role", "presentation").Result;

                return bo[1].InnerText.Replace("(geschätzt)", "");
            }
            return "";
        }

        private string GetRuntime(HtmlNode node)
        {
            if (node != null)
            {
                var tech = FindNodesByNode(node, "li", "role", "presentation").Result;

                return tech[0].InnerText.Replace("Laufzeit", "");
            }
            return "";
        }

        private string GetLocation(string text)
        {
            if (text.Contains("Drehorte"))
            {
                // Drehorte
                return text.Replace("Drehorte", "");
            }
            return "";
        }

        private string GetKnownAs(string text)
        {
            if (text.Contains("Auch bekannt als"))
            {
                // auch bekannt als
                return text.Replace("Auch bekannt als", "");
            }
            return "";
        }

        private string GetProductionCompany(string text)
        {
            if (text.Contains("Produktionsfirma"))
            {
                // Produktionsfirmen
                return text.Replace("Produktionsfirma", "");
            }
            return "";
        }

        private string GetProductionCompanies(string text)
        {
            if (text.Contains("Produktionsfirmen"))
            {
                // Produktionsfirmen
                return text.Replace("Produktionsfirmen", "");
            }
            return "";
        }

        #endregion

        #region Private Methodes

        private int GetCounter(HtmlNode node)
        {
            var counter = FindNodesByNode(node, "span", "class", "lister-current-last-item").Result.FirstOrDefault();
            if (counter != null)
            {
                var split = counter.NextSibling.InnerText.Split(" ");

                var test = split[13].Replace(".", "");

                bool isNumber = int.TryParse(test, out int number);
                if (isNumber)
                    return number;
            }
            return 0;
        }

        private async Task<List<HtmlNode>> FindNodesByNode(HtmlNode node, string a, string b, string c)
        {
            return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }

        private async Task<List<HtmlNode>> FindNodesByNodeEqual(HtmlNode node, string a, string b, string c)
        {
            return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Equals(c)).ToList();
        }

        private async Task<List<HtmlNode>> FindNodesByDocument(HtmlDocument document, string a, string b, string c)
        {
            return document.DocumentNode.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }
        #endregion
    }
}
