using OpenQA.Selenium;
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

    public void FirefoxDebug()
    {
        WebDriver = new FirefoxDriver();
    }

    public async Task<HtmlDocument> GetPageDocument(string url, int delay)
    {
        WebDriver.Navigate().GoToUrl(url);

        var hondaButton = WebDriver.FindElements(By.ClassName("fit-vehicle-list-view-text")).FirstOrDefault();

        if (hondaButton is not null)
        {
            hondaButton.Click();
        }

        if(url.Contains("store.steampowered.com"))
        {
            var change = WebDriver.FindElements(By.XPath("/html/body/div[1]/div[7]/div[6]/div/div[2]/div/div[1]/div[2]/select[3]/option[1]")).FirstOrDefault();
            if(change is not null)
            {
                change.Click();
                var button = WebDriver.FindElement(By.ClassName("btnv6_blue_hoverfade"));
                button.Click();
            }
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
