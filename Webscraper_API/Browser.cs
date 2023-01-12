using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Webscraper_API;
public class Browser
{
    public WebDriver WebDriver { get; set; }
    public Browser()
    {
        WebDriver = Firefox();
    }

    private FirefoxDriver Firefox()
    {
        FirefoxOptions options = new FirefoxOptions();
        options.AddArgument("--headless");
        options.AddArgument("--enable-precise-memory-info");
        options.AddArgument("--disable-popup-blocking");
        options.AddArgument("--blink-settings=imagesEnabled=false");
        options.AddArgument("--enable-javascript");
        options.AddArgument("--block-new-web-contents");
        FirefoxDriverService FService = FirefoxDriverService.CreateDefaultService();
        FService.HideCommandPromptWindow = true;

        return new FirefoxDriver(FService, options);
    }

    public FirefoxDriver FirefoxDebug()
    {
        return new FirefoxDriver();
    }

    public async Task<HtmlDocument> GetPageDocument(string url, int delay)
    {
        WebDriver.Navigate().GoToUrl(url);

        var hondaButton = WebDriver.FindElements(By.ClassName("fit-vehicle-list-view-text")).FirstOrDefault();

        if (hondaButton is not null)
        {
            hondaButton.Click();
        }
        await Task.Delay(delay);

        string page = ReplaceString(WebDriver.PageSource);
        //string page = WebDriver.PageSource;
        var doc = new HtmlDocument();
        doc.LoadHtml(page);
        return doc;
    }

    private readonly Regex Regex = new Regex(@"\\[uU]([0-9A-Fa-f]{4})");

    public string ReplaceString(string n)
    {
        n = n.Replace("&quot;", "")
            .Replace("&nbsp;", " ")
             .Replace("&amp;", "")
             .Replace("&szlig", "ß")
             .Replace("&Auml", "Ä")
             .Replace("auml", "ä")
             .Replace("Ouml", "Ö")
             .Replace("ouml", "ö")
             .Replace("Uuml", "Ü")
             .Replace("uuml", "ü")
             .Replace("&lt;", @$"< ")
             .Replace("&gt;", @$" > ");
        n = UnescapeUnicode(n);
        return n;
    }
    public string UnescapeUnicode(string str)
    {
        return Regex.Replace(str,
            match => ((char)int.Parse(match.Value.Substring(2),
                NumberStyles.HexNumber)).ToString());
    }

    public void CloseDriver()
    {
        WebDriver.Close();
        WebDriver.Quit();
    }

}
