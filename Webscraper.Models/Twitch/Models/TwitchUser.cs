namespace Webscraper.Models.Twitch.Models;

public class TwitchUser
{
    public string Id { get; set; } = string.Empty;
    public string ProfilUrl { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Follower { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string BannerUrl { get; set; } = string.Empty;
    public string Information { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TwitchGame[] LastStreamedGames { get; set; }
}
