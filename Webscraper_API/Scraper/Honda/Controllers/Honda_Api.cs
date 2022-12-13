using Webscraper_API.Interfaces;
using Webscraper_API.Scraper.Honda.Models;

namespace Webscraper_API.Scraper.Honda.Controllers
{
    public class Honda_Api : IHonda_Api
    {
        public async Task<string[]> GetMainCategoryUrls(HtmlDocument doc)
        {
            var main = FindNodesByDocument(doc, "div", "class", "cal-home-article").Result.FirstOrDefault();
            if (main is not null)
            {
                var list = FindNodesByNode(main, "li", "class", "home-part-list").Result;
                if (list is not null || list.Count > 0)
                {
                    List<string> urls = new List<string>();
                    foreach (var item in list)
                    {
                        var u = FindNodesByNode(item, "h3", "class", "home-part-head-category").Result.FirstOrDefault();
                        if (u is not null)
                        {
                            var split = u.InnerHtml.Split('"');
                            if (u.InnerHtml.Contains("href"))
                                urls.Add("https://www.hondapartsnow.com" + u.InnerHtml.Split('"')[1]);
                        }
                    }
                    return urls.ToArray();
                }
            }
            return null;
        }

        public async Task<CategoryUrl[]> GetCategoriesUrls(HtmlDocument doc)
        {
            // Categories
            //var main = FindNodesByDocument(doc, "ul", "class", "popular-list flex-row flex-wrap").Result.FirstOrDefault();
            var main = FindNodesByDocument(doc, "div", "class", "popular-category-content").Result.FirstOrDefault();
            if (main is not null)
            {
                var categoryMain = FindNodesByNode(main, "div", "class", "pr-category-content").Result.FirstOrDefault();
                var category = FindNodesByNode(categoryMain, "div", "class", "pr-cg-item-title pr-cg-item-has-bg").Result.FirstOrDefault();
                var list = FindNodesByNode(main, "a", "class", "popular-list-link").Result;
                if (list is not null || list.Count > 0)
                {
                    List<CategoryUrl> urls = new();
                    foreach (var item in list)
                    {
                        var split = item.OuterHtml.Split('"');
                        var sub = FindNodesByNode(item, "p", "class", "popular-list-name").Result.FirstOrDefault();
                        if (item.OuterHtml.Contains("href"))
                        {
                            urls.Add(new CategoryUrl()
                            {
                                Category = category.InnerText,
                                SubCategory = sub.InnerText,
                                Url = "https://www.hondapartsnow.com" + split[5]
                            });
                        }
                    }
                    return urls.ToArray();
                }
            }
            return null;
        }

        public async Task<string[]> GetAccessoriesUrls(HtmlDocument doc)
        {
            var main = FindNodesByDocument(doc, "ul", "class", "acc-category-list-wrap flex-row flex-wrap col-top").Result.FirstOrDefault();
            if (main is not null)
            {
                var list = FindNodesByNode(main, "a", "class", "acc-category-list-link").Result;
                if (list is not null || list.Count > 0)
                {
                    List<string> urls = new List<string>();
                    foreach (var item in list)
                    {
                        var split = item.OuterHtml.Split('"');
                        if (item.OuterHtml.Contains("href"))
                        {
                            urls.Add("https://www.hondapartsnow.com" + split[5]);
                        }
                    }
                    return urls.ToArray();
                }
            }
            return null;
        }

        public async Task<string[]> GetPartsUrls(HtmlDocument doc)
        {
            var main = FindNodesByDocument(doc, "div", "class", "pd-landing-loading ab-loading-container").Result.FirstOrDefault();
            if (main is not null)
            {
                var list = FindNodesByNode(main, "a", "class", "pd-ll-desc-url-n pd-ll-link").Result;
                if (list is not null || list.Count > 0)
                {
                    List<string> urls = new List<string>();
                    foreach (var item in list)
                    {
                        var split = item.OuterHtml.Split('"');
                        if (item.OuterHtml.Contains("href"))
                        {
                            urls.Add("https://www.hondapartsnow.com" + split[3]);
                        }
                    }
                    return urls.ToArray();
                }
            }
            return null;
        }

        public async Task<string[]> GetRelatedPartsUrls(HtmlDocument doc)
        {
            var main = FindNodesByDocument(doc, "div", "class", "part-number-wrap").Result.FirstOrDefault();

            List<string> relPartsUrls = new();
            var relatedParts = FindNodesByNode(main, "a", "class", "pn-part-list-link").Result;
            foreach (var relparts in relatedParts)
            {
                var relpart = relparts.OuterHtml.Split('"')[5];
                relPartsUrls.Add("https://www.hondapartsnow.com" + relpart);
            }
            return relPartsUrls.ToArray();
        }

        public async Task<NewParts> GetPart(HtmlDocument doc, CategoryUrl category)
        {
            var main = FindNodesByDocument(doc, "div", "class", "part-number-wrap").Result.FirstOrDefault();
            if (main is not null)
            {
                NewParts parts = new();

                // Images
                var images = FindNodesByNode(main, "div", "class", "img-carousel-img").Result;
                List<string> imageUrls = new();
                foreach (var image in images)
                {
                    var split = image.InnerHtml.Split('"');
                    var i = "https://www.hondapartsnow.com" + split[7];
                    imageUrls.Add(i);
                }

                // ReferanceImageUrl
                var refImages = FindNodesByNode(main, "div", "class", "pn-dia-pic pn-single-dia-pic").Result.FirstOrDefault();
                var refi = string.Empty;
                if (refImages is not null)
                {
                    var split2 = refImages.InnerHtml.Split('"');
                    refi = "https://www.hondapartsnow.com" + split2[3].Replace("medium", "large");
                }

                // Related Parts
                List<string> relPartsUrls = new();
                var relatedParts = FindNodesByNode(main, "a", "class", "pn-part-list-link").Result;
                foreach (var relparts in relatedParts)
                {
                    var relpart = relparts.OuterHtml.Split('"')[5];
                    relPartsUrls.Add("https://www.hondapartsnow.com" + relpart);
                }

                // Product Specifications
                var table = FindNodesByNode(main, "table", "class", "pn-spec-list").Result.FirstOrDefault();
                if (table is not null)
                {
                    var td = FindNodesByNode(table, "td", "", "").Result;

                    for (int i = 0; i < td.Count; i++)
                    {
                        switch (td[i].InnerHtml)
                        {
                            case "Brand":
                                parts.Brand = td[i + 1].InnerHtml;
                                break;
                            case "Manufacturer Part Number":
                                parts.ID = td[i + 1].InnerHtml;
                                break;
                            case "Part Description":
                                parts.PartDescription = td[i + 1].InnerHtml;
                                break;
                            case "Other Names":
                                parts.OtherNames = td[i + 1].InnerHtml;
                                break;
                            case "Item Dimensions":
                                parts.ItemDimensions = td[i + 1].InnerHtml;
                                break;
                            case "Item Weight":
                                parts.ItemWeight = td[i + 1].InnerHtml;
                                break;
                            case "Condition":
                                parts.Condition = td[i + 1].InnerHtml;
                                break;
                            case "Fitment Type":
                                parts.FitmentType = td[i + 1].InnerHtml;
                                break;
                            case "Manufacturer":
                                parts.Manufaktur = td[i + 1].InnerHtml;
                                break;
                            case "SKU":
                                parts.SKU = td[i + 1].InnerHtml;
                                break;
                        }
                    }
                }

                // Part Fitment

                List<PartFitment> pfList = new();
                var partFitmentTable = FindNodesByNode(main, "table", "class", "fit-vehicle-list-table").Result.FirstOrDefault();
                var partFitmentTBody = FindNodesByNode(partFitmentTable, "tbody", "", "").Result.FirstOrDefault();
                var partFitmentList = FindNodesByNode(partFitmentTBody, "tr", "", "").Result;
                foreach (var item in partFitmentList)
                {
                    var split3 = item.InnerHtml.Split('"');
                    // 3 7 11
                    pfList.Add(new PartFitment()
                    {
                        ID = split3[3],
                        YearMakeModel = split3[3],
                        BodyTrim = split3[7],
                        EmissionTransmission = split3[11]
                    });
                }

                // All Details
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    if (i < imageUrls.Count - 1)
                        parts.ImageUrls += imageUrls[i] + ";";
                    else
                        parts.ImageUrls += imageUrls[i];
                }

                for (int i = 0; i < relPartsUrls.Count; i++)
                {
                    if (i < relPartsUrls.Count - 1)
                        parts.RelatedPartsUrls += relPartsUrls[i] + ";";
                    else
                        parts.RelatedPartsUrls += relPartsUrls[i];
                }


                parts.Url = category.Url;
                parts.Category = category.Category;
                parts.SubCategory = category.SubCategory;
                parts.ReferanceImageUrl = refi;

                //parts.PartFitments = pfList.ToArray();

                return parts;

            }
            return null;
        }

        public async Task<Accessories> GetAccessories(string url, HtmlDocument doc)
        {
            var main = FindNodesByDocument(doc, "div", "class", "acc-part-number container").Result.FirstOrDefault();
            if (main is not null)
            {
                Accessories acc = new Accessories();

                // Name
                var accName = FindNodesByNode(main, "h1", "class", "acc-pn-detail-main-title").Result.FirstOrDefault();

                // Part ID
                var partNo = FindNodesByNode(main, "div", "class", "acc-pn-detail-sub-desc").Result.FirstOrDefault();

                // Description as Array
                var desc = FindNodesByNode(main, "div", "class", "html-show-more-content").Result.FirstOrDefault();
                var descList = FindNodesByNode(desc, "li", "", "").Result;

                List<string> descText = new();

                foreach (var d in descList)
                {
                    descText.Add(d.InnerText);
                }

                // This Part Fits
                var partFits = FindNodesByNode(main, "table", "class", "fit-vehicle-list-table").Result.FirstOrDefault();
                var partFitsTBody = FindNodesByNode(partFits, "tbody", "", "").Result.FirstOrDefault();
                var partFitsTr = FindNodesByNode(partFitsTBody, "tr", "", "").Result;

                List<PartFits> pfs = new List<PartFits>();
                foreach (var trs in partFitsTr)
                {
                    var tds = FindNodesByNode(trs, "", "", "").Result;
                    pfs.Add(new PartFits()
                    {
                        Make = tds[0].InnerText,
                        Model = tds[1].InnerText,
                        Year = tds[2].InnerText,
                        BodyTrim = tds[3].InnerText
                    });
                }


                acc.ID = partNo.InnerText.Split(" ")[1];
                acc.Url = url;
                acc.Name = accName.InnerText;
                acc.Description = descText.ToArray();
                acc.PartFits = pfs.ToArray();


            }
            return null;
        }


        private async Task<List<HtmlNode>> FindNodesByNode(HtmlNode node, string a, string b, string c)
        {
            return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }
        private async Task<List<HtmlNode>> FindNodesByDocument(HtmlDocument document, string a, string b, string c)
        {
            return document.DocumentNode.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }
    }
}
