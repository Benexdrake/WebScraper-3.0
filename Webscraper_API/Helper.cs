namespace Webscraper_API;

public class Helper
{
    public static double Percent(int n, int max)
    {
        return Math.Round((double)(100 * n) / max, 2);
    }

    public static async Task<List<HtmlNode>> FindNodesByNode(HtmlNode node, string a, string b, string c)
    {
        return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
    }
    public static async Task<List<HtmlNode>> FindNodesByDocument(HtmlDocument document, string a, string b, string c)
    {
        return document.DocumentNode.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
    }
}
