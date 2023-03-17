namespace Webscraper.Models.Steam.Models;

public class Price
{
    public string Title { get; set; }
    public double OriginalPrice { get; set; }
    public double DiscountPrice { get; set; }
    public bool IsFree { get; set; }
}
