namespace Webscraper_API.Scraper.Pokemons.Models
{
    public class Pokemon
    {
        public string Id { get; set; }
        public int Nr { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public string Category { get; set; }
        public string SkillName { get; set; }
        public string SkillDescription { get; set; }
        public string Sex { get; set; }
        public string Type { get; set; }
        public string Weakness { get; set; }
        public int KP { get; set; }
        public int Attack { get; set; }
        public int Defensiv { get; set; }
        public int SPAttack { get; set; }
        public int SPDefensiv { get; set; }
        public int Initiative { get; set; }
        public bool HasVersions { get; set; }
    }
}
