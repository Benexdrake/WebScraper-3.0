namespace Webscraper_API.Scraper.Pokemons.Models
{
    public class Pokemon
    {
        public string Id { get; set; } = string.Empty;
        public int Nr { get; set; }
        public string Url { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SkillName { get; set; } = string.Empty;
        public string SkillDescription { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Weakness { get; set; } = string.Empty;
        public int KP { get; set; }
        public int Attack { get; set; }
        public int Defensiv { get; set; }
        public int SPAttack { get; set; }
        public int SPDefensiv { get; set; }
        public int Initiative { get; set; }
        public bool HasVersions { get; set; }
    }
}
