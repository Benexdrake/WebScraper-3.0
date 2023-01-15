namespace Webscraper_API.Scraper.Apple.Iphone.Models;

public class IPhone
{
    //public int Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public string ImageUrl { get; set; }
    public Network Network { get; set; }
    public Launch Launch { get; set; }
    public Body Body { get; set; }
    public Display Display { get; set; }
    public Platform Platform { get; set; }
    public Memory Memory { get; set; }
    public List<Camera> Camera { get; set; } = new List<Camera>();
    public Sound Sound { get; set; }
    public Comms Comms { get; set; }
    public Features Features { get; set; }
    public Battery Battery { get; set; }
    public Misc Misc { get; set; }
    public Tests Tests { get; set; }
}
