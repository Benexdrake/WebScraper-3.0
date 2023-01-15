
using OpenQA.Selenium.DevTools;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Webscraper_API.Scraper.Apple.Iphone.Models;

namespace Webscraper_API.Scraper.Apple.Iphone.Controllers;

public class I_API
{
    private readonly Browser _browser;
    public I_API(IServiceProvider service)
    {
        _browser = service.GetRequiredService<Browser>();
    }

    public async Task<string[]> GetPhoneUrlsAsync(string url)
    {
        var doc = _browser.GetPageDocument(url, 1000).Result;

        var main = Helper.FindNodesByDocument(doc, "div", "class", "section-body").Result.FirstOrDefault();

        if (main != null) 
        {
            var urls = new List<string>();
            var list = Helper.FindNodesByNode(main, "li","","").Result;
            if(list.Count > 0) 
            {
                foreach (var item in list) 
                {
                    string mainUrl = "https://www.gsmarena.com";
                    var split = item.InnerHtml.Split('"');
                    urls.Add(mainUrl +"/" +split[1]);
                    
                }
                return urls.ToArray();
            }
        }
        return new string[0];
    }

    public async Task<IPhone> GetPhoneAsync(string url)
    {
        var doc = _browser.GetPageDocument(url, 1000).Result;

        var main = Helper.FindNodesByDocument(doc,"div","class", "main main-review right l-box col").Result.FirstOrDefault();
        if(main != null)
        {
            var iphone = new IPhone();
            iphone.Network = new();
            iphone.Battery = new();
            iphone.Launch = new();
            iphone.Body = new();
            iphone.Display = new();
            iphone.Platform = new();
            iphone.Memory = new();
            iphone.Sound = new();
            iphone.Comms = new();
            iphone.Misc = new();
            iphone.Tests = new();

            var header = Helper.FindNodesByNode(main, "div", "class", "review-header").Result.FirstOrDefault();
            var specMain = Helper.FindNodesByNode(main, "div", "id", "specs-list").Result.FirstOrDefault();

            var specList = Helper.FindNodesByNode(specMain, "table", "cellspacing", "0").Result;

            // Image
            iphone.ImageUrl = Helper.FindNodesByNode(header,"div","class", "specs-photo-main").Result.FirstOrDefault().InnerHtml.Split('"')[5];

            // Name
            iphone.Name = Helper.FindNodesByNode(header, "h1", "class", "specs-phone-name-title").Result.FirstOrDefault().InnerText;

            // Version
            var versions = Helper.FindNodesByNode(specMain, "p", "data-spec", "comment").Result.FirstOrDefault();
            if(versions is not null)
            iphone.Comment = versions.InnerText;

            foreach (var specs in specList)
            {
                var trs = Helper.FindNodesByNode(specs, "tr", "", "").Result;

                // Network
                if (specs.InnerHtml.Contains("scope=\"row\">Network"))
                {
                    
                    var networkList = Helper.FindNodesByNode(specs, "tr", "class", "tr-hover").Result;
                    networkList.AddRange(Helper.FindNodesByNode(specs, "tr", "class", "tr-toggle").Result);

                    iphone.Network.Technology = FilterData(networkList,"Technology", "data-spec-optional");
                    iphone.Network.Bands2G = FilterData(networkList, "2G bands", "data-spec-optional");
                    iphone.Network.Bands3G = FilterData(networkList, "3G bands", "data-spec-optional");
                    iphone.Network.Bands4G = FilterData(networkList, "4G bands", "data-spec-optional");
                    iphone.Network.Bands5G = FilterData(networkList, "5G bands", "data-spec-optional");
                    iphone.Network.Speed = FilterData(networkList, "Speed", "data-spec-optional");
                }
                // Launch
                if (specs.InnerHtml.Contains("scope=\"row\">Launch"))
                {
                    
                    iphone.Launch.Announced = FilterData(trs, "year", "launchother");
                    iphone.Launch.Status = FilterData(trs, "status", "launchother");
                }
                // Body
                if (specs.InnerHtml.Contains("scope=\"row\">Body"))
                {
                    
                    iphone.Body.Dimension = FilterData(trs, "dimension", "bodyother");
                    iphone.Body.Weight = FilterData(trs, "weight", "bodyother");
                    iphone.Body.Build = FilterData(trs, "build", "bodyother");
                    iphone.Body.Sim = FilterData(trs, "sim", "bodyother");
                }
                // Display
                if (specs.InnerHtml.Contains("scope=\"row\">Display"))
                {
                    
                    iphone.Display.Type = FilterData(trs, "type", "displayother");
                    iphone.Display.Size = FilterData(trs, "size", "displayother");
                    iphone.Display.Resolution = FilterData(trs, "resolution", "displayother");
                    iphone.Display.Protection = FilterData(trs, "protection", "displayother");
                }
                // Platform
                if (specs.InnerHtml.Contains("scope=\"row\">Platform"))
                {
                    
                    iphone.Platform.OS = FilterData(trs, "os", "platformother");
                    iphone.Platform.Chipset = FilterData(trs, "chipset", "platformother");
                    iphone.Platform.CPU = FilterData(trs, "cpu", "platformother");
                    iphone.Platform.GPU = FilterData(trs, "gpu", "platformother");
                }
                // Memory
                if (specs.InnerHtml.Contains("scope=\"row\">Memory"))
                {
                    
                    iphone.Memory.CardSlot = CheckFilter(trs, "cardslot", "memoryother");

                    iphone.Memory.Internal = FilterData(trs, "internal", "memoryother");
                }
                // Camera
                if (specs.InnerHtml.Contains("Main Camera"))
                {
                    var camera = Helper.FindNodesByNode(trs[0],"td","class", "ttl").Result.FirstOrDefault();
                    var t = camera.InnerText;
                    iphone.Camera.Add(new Camera()
                    {
                        Place = "Main Camera",
                        CameraType = t,
                        CameraSpecs = FilterData(trs,t,"cameraother"),
                        Features = FilterData(trs, "feature", "cameraother"),
                        Video = FilterData(trs, "video", "cameraother")
                    });
                    continue;
                }
                if (specs.InnerHtml.Contains("Selfie camera"))
                {
                    var camera = Helper.FindNodesByNode(trs[0], "td", "class", "ttl").Result.FirstOrDefault();
                    var t = camera.InnerText;
                    iphone.Camera.Add(new Camera()
                    {
                        Place = "Selfie Camera",
                        CameraType = t,
                        CameraSpecs = FilterData(trs, t, "cameraother"),
                        Features = FilterData(trs, "feature", "cameraother"),
                        Video = FilterData(trs, "video", "cameraother")
                    });
                }
                // Sound
                if (specs.InnerHtml.Contains("scope=\"row\">Sound"))
                {
                    
                    iphone.Sound.Loadspeaker = FilterData(trs, "loudspeaker", "soundother");
                    iphone.Sound.Jack35 = CheckFilter(trs, "3,5mm jack", "soundother");
                }
                // Comms
                if (specs.InnerHtml.Contains("scope=\"row\">Comms"))
                {
                    
                    iphone.Comms.Wlan = FilterData(trs, "wlan", "commsother");
                    iphone.Comms.Bluetooth = FilterData(trs, "bluetooth", "commsother");
                    iphone.Comms.Positioning = FilterData(trs, "positioning", "commsother"); //  xxxxxxxxxxxxxxxxxxxxxxx
                    iphone.Comms.NFC = CheckFilter(trs, "nfc", "commsother");
                    iphone.Comms.Radio = CheckFilter(trs, "radio", "commsother");
                    iphone.Comms.Usb = FilterData(trs, "usb", "commsother");
                }
                // Features
                if (specs.InnerHtml.Contains("scope=\"row\">Features"))
                {
                    iphone.Features= new();
                    iphone.Features.Sensors = FilterData(trs, "sensors", "featureother");
                }
                // Battery
                if (specs.InnerHtml.Contains("scope=\"row\">Battery"))
                {
                    
                    iphone.Battery.Type = FilterData(trs,"type","batteryother");
                    iphone.Battery.Charging = FilterData(trs,"charging","batteryother");
                }
                // Misc
                if (specs.InnerHtml.Contains("scope=\"row\">Misc"))
                {
                    
                    iphone.Misc.Colors = FilterData(trs,"colors","miscother");
                    iphone.Misc.Models = FilterData(trs,"models","miscother");
                    iphone.Misc.SAR = FilterData(trs,"sar-us","miscother");
                    iphone.Misc.SAREU = FilterData(trs, "sar-eu", "miscother");
                    iphone.Misc.Price = FilterData(trs,"price","miscother");
                }
                // Tests
                if (specs.InnerHtml.Contains("scope=\"row\">Tests"))
                {   
                    
                    iphone.Tests.Performance = FilterData(trs, "tbench", "testsother"); // xxxxxxxxxxxxxxxxxxx
                    iphone.Tests.Display = FilterData(trs,"Display","testsother");
                    iphone.Tests.Camera = FilterData(trs,"Camera","testsother");            // xxxxxxxxxxxxxxxxxxxx
                    iphone.Tests.Loudspeaker = FilterData(trs,"Loudspeaker","testsother");  // xxxxxxxxxxxxxxxxxxxx
                    iphone.Tests.BatteryLife = FilterData(trs, "batlife", "testsother"); // xxxxxxxxxxxxxxxxxxxx
                }
            }

            return iphone;
        }

        return null;
    }

    private string FilterData(List<HtmlNode> nodes, string name, string search)
    {
        string filter = string.Empty;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].InnerHtml.Contains(name))
            {
                var nfo = Helper.FindNodesByNode(nodes[i], "td", "class", "nfo").Result.FirstOrDefault();
                filter = nfo.InnerText;

                for (int j = i + 1; j < nodes.Count; j++)
                {
                    if (nodes[j].OuterHtml.Contains(search))
                    {
                        nfo = Helper.FindNodesByNode(nodes[j], "td", "class", "nfo").Result.FirstOrDefault();
                        filter += ";" + nfo.InnerText;
                    }
                    else
                        break;
                }
            }
        }
        return filter.Trim();
    }

    private bool CheckFilter(List<HtmlNode> nodes, string name, string search)
    {
        if(FilterData(nodes,name,search).Contains("Yes"))
            return true;
        else
            return false;
    }

}
