namespace Webscraper.Models.Steam.Models;
public class SteamUser
{
    public string Id { get; set; }
    public string ProfilUrl { get; set; }
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    public string ProfilText { get; set; }
    public int Level { get; set; }
    public string BadgeIconUrl { get; set; }
    public string[] Games { get; set; }
    public int Reviews { get; set; }
    public int GamesOnWishlist { get; set; }
    public int Trophys { get; set; }
    public int Platin { get; set; }
}