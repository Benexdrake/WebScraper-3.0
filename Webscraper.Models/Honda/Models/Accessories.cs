namespace Webscraper.Models.Honda.Models
{
    public class Accessories
    {
        public string ID { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string[] Description { get; set; }
        public PartFits[] PartFits { get; set; }
    }
}
