namespace Webscraper_API.Scraper.Dota2.Models;
public class Ability
{
    public int id { get; set; }
    public string name { get; set; }
    public string name_loc { get; set; }
    public string desc_loc { get; set; }
    public string lore_loc { get; set; }
    public string[] notes_loc { get; set; }
    public string shard_loc { get; set; }
    public string scepter_loc { get; set; }
    public int type { get; set; }
    public string behavior { get; set; }
    public int target_team { get; set; }
    public int target_type { get; set; }
    public int flags { get; set; }
    public int damage { get; set; }
    public int immunity { get; set; }
    public int dispellable { get; set; }
    public int max_level { get; set; }
    public int[] cast_ranges { get; set; }
    public float[] cast_points { get; set; }
    public float[] channel_times { get; set; }
    public float[] cooldowns { get; set; }
    public float[] durations { get; set; }
    public int[] damages { get; set; }
    public int[] mana_costs { get; set; }
    public object[] gold_costs { get; set; }
    public Special_Values[] special_values { get; set; }
    public bool is_item { get; set; }
    public bool ability_has_scepter { get; set; }
    public bool ability_has_shard { get; set; }
    public bool ability_is_granted_by_scepter { get; set; }
    public bool ability_is_granted_by_shard { get; set; }
    public int item_cost { get; set; }
    public int item_initial_charges { get; set; }
    public long item_neutral_tier { get; set; }
    public int item_stock_max { get; set; }
    public int item_stock_time { get; set; }
    public int item_quality { get; set; }
}
