namespace Webscraper_API.Scraper.Dota2.Models;
public class Hero
{
    public int id { get; set; }
    public string name { get; set; }
    public string imageUrl { get; set; }
    public string videoUrl { get; set; }
    public int order_id { get; set; }
    public string name_loc { get; set; }
    public string bio_loc { get; set; }
    public string hype_loc { get; set; }
    public string npe_desc_loc { get; set; }
    public int str_base { get; set; }
    public float str_gain { get; set; }
    public int agi_base { get; set; }
    public float agi_gain { get; set; }
    public int int_base { get; set; }
    public float int_gain { get; set; }
    public int primary_attr { get; set; }
    public int complexity { get; set; }
    public int attack_capability { get; set; }
    public int[] role_levels { get; set; }
    public int damage_min { get; set; }
    public int damage_max { get; set; }
    public float attack_rate { get; set; }
    public int attack_range { get; set; }
    public int projectile_speed { get; set; }
    public float armor { get; set; }
    public int magic_resistance { get; set; }
    public int movement_speed { get; set; }
    public float turn_rate { get; set; }
    public int sight_range_day { get; set; }
    public int sight_range_night { get; set; }
    public int max_health { get; set; }
    public float health_regen { get; set; }
    public int max_mana { get; set; }
    public float mana_regen { get; set; }
    public Ability[] abilities { get; set; }
    public Talent[] talents { get; set; }
}
