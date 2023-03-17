namespace Webscraper.Models.Amazon.Models;

public class Product
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public string ReleaseDate { get; set; }
    public bool Available { get; set; }
}
