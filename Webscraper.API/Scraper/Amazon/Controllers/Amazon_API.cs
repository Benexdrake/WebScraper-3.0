namespace Webscraper.API.Scraper.Amazon.Controllers;

public class Amazon_API
{
    private readonly Browser _browser;
    public Amazon_API(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }

    public async Task<Product> GetProduct(string url)
    {
        _browser.WebDriver = _browser.FirefoxDebug();

        var doc = _browser.GetPageDocument(url, 1000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "id", "ppd").Result.FirstOrDefault();
        if (main is not null)
        {
            Product p = new();
            // ID
            var split = url.Split('/'); // 5
            p.Id = split[5];

            // Product Name
            var productName = Helper.FindNodesByNode(main, "span", "class", "a-size-large product-title-word-break").Result.FirstOrDefault();
            p.Title = productName.InnerText.Trim();

            // Price
            var price = Helper.FindNodesByNode(main, "span", "class", "a-offscreen").Result.FirstOrDefault();
            p.Price = double.Parse(price.InnerText.Substring(0, price.InnerText.Length - 1));

            // Product Image
            var image = Helper.FindNodesByNode(main, "img", "class", "a-dynamic-image a-stretch-vertical").Result.FirstOrDefault();
            var imageSplit = image.OuterHtml.Split('"'); // 5
            p.ImageUrl = imageSplit[5];

            // Description
            var description = Helper.FindNodesByNode(main, "div", "id", "feature-bullets").Result.FirstOrDefault();
            var descriptionItems = Helper.FindNodesByNode(description, "span", "class", "a-list-item").Result;
            for (int i = 0; i < descriptionItems.Count; i++)
            {
                if (i < descriptionItems.Count - 1)
                    p.Description += descriptionItems[i].InnerText.Trim() + "." + Environment.NewLine;
                else
                    p.Description += descriptionItems[i].InnerText.Trim() + ".";
            }

            // Release Date
            var releaseDate = Helper.FindNodesByNode(main, "div", "id", "availability").Result.FirstOrDefault();
            p.ReleaseDate = releaseDate.InnerText.Replace("Dieser Artikel erscheint am", "").Replace("Jetzt vorbestellen.", "").Trim();

            // Buy Button - Bool
            var buyButton = Helper.FindNodesByNode(main, "input", "id", "buy-now-button").Result.FirstOrDefault();
            if (buyButton is not null)
            {
                p.Available = true;
            }

            return p;
        }
        return null;
    }

}
