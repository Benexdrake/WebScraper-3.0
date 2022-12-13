namespace Webscraper_API;

public class Helper
{
    public static double Percent(int n, int max)
    {
        return Math.Round((double)(100 * n) / max, 2);
    }

    
}
