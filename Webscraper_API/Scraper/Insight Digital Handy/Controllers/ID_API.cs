using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Webscraper_API.Scraper.Insight_Digital_Handy.Models;

namespace Webscraper_API.Scraper.Insight_Digital_Handy.Controllers
{
    public class ID_API : IID_API
    {
        private readonly Browser _browser;
        public ID_API(IServiceProvider service)
        {
            _browser = service.GetRequiredService<Browser>();
        }

        public async Task<string[]> GetHandyUrls()
        {
            var doc = _browser.GetPageDocument("https://www.inside-digital.de/handy-bestenliste/inside-digital-ranking/1", 1000).Result;

            var main = Helper.FindNodesByDocument(doc, "div", "class", "main-pagination").Result.FirstOrDefault();

            if (main is not null)
            {
                 List<string> urls = new List<string>();

                var max = Helper.FindNodesByNode(main, "a", "class", "two-digits").Result;

                int n = int.Parse(max[max.Count - 1].InnerText);

                for (int i = 1; i <= n; i++)
                {
                    string url = $"https://www.inside-digital.de/handy-bestenliste/inside-digital-ranking/{i}";
                    doc = _browser.GetPageDocument(url, 1000).Result;
                    var block = Helper.FindNodesByDocument(doc, "div", "class", "td_block_inner").Result.FirstOrDefault();
                    if(block is not null)
                    {
                        var list = Helper.FindNodesByNode(block, "div", "class", "td-block-span12").Result;
                        foreach (var l in list)
                        {
                            var u = Helper.FindNodesByNode(l, "a", "href", "https://www.inside-digital.de").Result.FirstOrDefault();
                            urls.Add(u.OuterHtml.Split('"')[1]);
                        }
                    }
                }
                return urls.ToArray();
            }
            return null;
        }

        public async Task<Handy> GetHandyAsync(string url)
        {
            var doc = _browser.GetPageDocument(url, 1000).Result;

            var main = Helper.FindNodesByDocument(doc, "table", "class", "datasheet_table").Result;
            if (main is not null)
            {
                var handy = new Handy();
                var display = new Display();
                var casing = new Casing();
                var hardware = new Hardware();
                var connectivity = new Connectivity();
                var connectTrans = new ConnectionTransmission();
                var misc = new Miscellaneous();

                var image = Helper.FindNodesByDocument(doc,"img","loading","eager").Result.FirstOrDefault();
                var imageSplit = image.OuterHtml.Split('"');

                var name = Helper.FindNodesByDocument(doc, "h1", "class", "entry-title td-page-title").Result.FirstOrDefault();

                List<HtmlNode> dataSheets = new List<HtmlNode>();

                for (int i = 0; i < main.Count; i++)
                {
                    dataSheets.AddRange(Helper.FindNodesByNode(main[i], "tr", "class", "s_field").Result);
                }

                var maxCameras = dataSheets.Where(x => x.InnerHtml.Contains("Rückseitige Kamera") || x.InnerHtml.Contains("Fronthauptkamera")).Count();
                var cameras = new Camera[maxCameras];
                for (int i = 0; i < cameras.Length; i++)
                {
                    cameras[i] = new();
                }
                int cameraCounter = 0;

                bool displayTyp = false;

                for (int i = 0; i < dataSheets.Count; i++)
                {
                    var left = Helper.FindNodesByNode(dataSheets[i], "span", "class", "db_value").Result.FirstOrDefault();
                    if(left is null)
                    {
                        left = Helper.FindNodesByNode(dataSheets[i], "span", "class", "td_bezeichnung").Result.FirstOrDefault();
                        if(left is null)
                            left = Helper.FindNodesByNode(dataSheets[i], "td", "class", "td_bezeichnung").Result.FirstOrDefault();
                        if (left is not null)
                        {
                            if (left.InnerText.Trim().Equals("Typ"))
                                displayTyp = true;
                        }
                    }

                    if(left is not null)
                    {
                        // Looking for Left and putting data from Right.

                        var right = Helper.FindNodesByNode(dataSheets[i], "td", "class", "td_value").Result.FirstOrDefault();
                        if (right is not null)
                        {
                            string leftText = left.InnerText.Trim();
                            string rightText = string.Empty;
                            var icon = Helper.FindNodesByNode(right, "i", "class", "fas fa-").Result.FirstOrDefault();
                            if (icon is not null)
                            {
                                var split = icon.OuterHtml.Split('"');

                                if(split.Length > 4)
                                    rightText = split[3].Trim();
                            }
                            else
                            {
                                rightText = right.InnerText.Trim();
                            }

                            //Generally
                            if (leftText.Contains("Hersteller"))
                                handy.Manufacturer = rightText;
                            else if (leftText.Contains("Modell"))
                                handy.Model = rightText;
                            else if (leftText.Contains("Variant")) // Will net...
                                handy.Variants = rightText;
                            else if (leftText.Contains("Verfügbar"))
                                handy.Accessible = rightText;
                            else if (leftText.Contains("Marktstart in Deutschland"))
                                handy.LaunchGermany = rightText;
                            else if (leftText.Contains("Einführungspreis"))
                                handy.Prices = rightText;
                            else if (leftText.Contains("Betriebssystem (OS)"))
                                handy.OS = rightText;
                            else if (leftText.Contains("(UI)"))
                                handy.UI = rightText;

                            // Display
                            else if (leftText.Contains("Pixeldichte")) 
                                display.PixelDensity = rightText;
                            else if(leftText.Contains("Auflösung B x H"))
                                display.ResolutionWH = rightText;
                            else if (leftText.Contains("Diagonale Zoll"))
                                display.DiagonalInches = rightText;
                            else if (leftText.Contains("Diagonale mm"))
                                display.DiagonalMm = rightText;
                            else if (leftText.Equals("Typ") && displayTyp)
                            {
                                display.Type = rightText;
                                displayTyp = false;
                            }
                            else if (leftText.Contains("Bildwiederholfrequenz"))
                                display.RefreshRate = rightText;
                            else if (leftText.Contains("Material"))
                                display.Material = rightText;
                            else if (leftText.Contains("Display-Format"))
                                display.DisplayFormat = rightText;

                            // Gehäuse
                            else if (leftText.Contains("Gehäusematerial"))
                                casing.CaseMaterial = rightText;
                            else if (leftText.Contains("Höhe  x Breite x Tiefe"))
                            {
                                //rightText = Helper.FindNodesByNode(dataSheets[i+1], "td", "class", "td_value").Result.FirstOrDefault().InnerText.Trim();
                                casing.CaseFormatHWD = rightText;
                            }
                            else if (leftText.Contains("Schutzart"))
                                casing.DegreeOfProtection = rightText;
                            else if (leftText.Contains("Schutz gegen"))
                                casing.ProtectionAgainst = rightText;
                            else if (leftText.Contains("Gewicht"))
                                casing.Weight = rightText;
                            else if (leftText.Contains("Farben"))
                                casing.Color = rightText;

                            // Hardware
                            else if (leftText.Contains("Prozessor / CPU"))
                                hardware.Processor = rightText;
                            else if (leftText.Contains("Arbeitsspeicher"))
                                hardware.RAM = rightText;
                            else if (leftText.Contains("Akku Kapazität"))
                                hardware.BatteryCapacity = rightText;
                            else if (leftText.Contains("Ladegeschwindigkeit"))
                                hardware.BatteryLoadingSpeed = rightText;
                            else if (leftText.Contains("Induktives Laden"))
                                hardware.BatteryInductiveCharging = rightText;
                            else if (leftText.Contains("interner Speicher"))
                                hardware.StorageInternal = rightText;
                            else if (leftText.Contains("SD-Kartenslot"))
                                hardware.SDCardSlot = rightText;
                            else if (leftText.Contains("SIM-Kartenslot"))
                                hardware.SimCardSlot = rightText;
                            else if (leftText.Contains("Mehrfach SIM"))
                                hardware.MultipleSim = rightText;
                            else if (leftText.Contains("Einschränkung"))
                                hardware.RestrictionHybridSubject = rightText;
                            else if (leftText.Contains("Sensoren"))
                                hardware.Sensors = rightText;
                            else if (leftText.Contains("Hardware-Tastatur"))
                                hardware.HardwareKeyboard = rightText;

                            // Konnektivität
                            else if (leftText.Contains("LTE"))
                                connectivity.Connection4G = rightText;
                            else if (leftText.Contains("5G"))
                                connectivity.Connection5G = rightText;
                            else if (leftText.Contains("WLAN"))
                                connectivity.WirelessInternetAccess = rightText;
                            else if (leftText.Contains("Standard"))
                                connectivity.Standard = rightText;
                            else if (leftText.Contains("Band"))
                                connectivity.Band = rightText;

                            // Anschlüsse und Übertragung
                            else if (leftText.Contains("USB-Port"))
                                connectTrans.UsbPort = rightText;
                            else if (leftText.Contains("Typ"))
                                connectTrans.UsbType = rightText;
                            else if (leftText.Contains("Lightning-Connector"))
                                connectTrans.LightningConnector = rightText;
                            else if (leftText.Contains("Bluetooth"))
                                connectTrans.Bluetooth = rightText;
                            else if (leftText.Contains("Audio-/ Kopfhörerbuchse"))
                                connectTrans.AudioHeadset = rightText;
                            else if (leftText.Contains("NFC"))
                                connectTrans.Nfc = rightText;
                            else if (leftText.Contains("Radio"))
                                connectTrans.Radio = rightText;



                            // Sonstiges
                            else if (leftText.Contains("Besonderheiten"))
                                misc.Particularities = rightText;
                            else if (leftText.Contains("Lieferumfang"))
                                misc.ScopeOfDelivery = rightText;


                            // Kamera - da muss ich mir noch was einfallen lassen

                            try
                            {
                                int j = 0;
                                for (int x = 0; x < maxCameras; x++)
                                {
                                    string n = string.Empty;
                                    if(x > 0)
                                    {
                                        j = x + 1;
                                        n = " " + j;
                                    }

                                    if(leftText.Equals($"Rückseitige Kamera{n}"))
                                        cameras[x].CameraType = rightText;
                                    else if (leftText.Equals($"Kameraauflösung{n}"))
                                        cameras[x].Resolution = rightText;
                                    else if (leftText.Equals($"Sensor{n}"))
                                        cameras[x].Sensor = rightText;
                                    else if (leftText.Equals($"Blende{n}") || leftText.Equals($"Kamerablende{n}"))
                                        cameras[x].Cover = rightText;
                                    else if (leftText.Equals($"Bildstabilisator{n}"))
                                        cameras[x].ImageStabilizer = rightText;
                                    else if (leftText.Equals($"Digitaler Zoom{n}"))
                                        cameras[x].DigitalZoom = rightText;
                                    else if (leftText.Equals($"Optischer Zoom{n}"))
                                        cameras[x].OpticalZoom = rightText;
                                    else if (leftText.Equals($"Videoauflösung{n}"))
                                        cameras[x].VideoResolution = rightText;
                                    else if (leftText.Equals($"Aufnahmegeschwindigkeit{n}"))
                                        cameras[x].RecordingSpeed = rightText;
                                    else if (leftText.Equals($"Blitz / Fotoleuchte{n}"))
                                        cameras[x].FlashPhotoLight = rightText;

                                    if (x == maxCameras - 1)
                                    {

                                        // Frontkamera
                                        if (leftText.Equals($"Fronthauptkamera"))
                                            cameras[x].CameraType = rightText;
                                        else if (leftText.Equals($"Auflösung"))
                                            cameras[x].Resolution = rightText;
                                        else if (leftText.Equals($"Videoauflösung"))
                                                cameras[x].VideoResolution = rightText;
                                    }
                                }
                                j = 0;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            
                        }
                    }
                }

                
                var urlSplit = url.Split('/');


                handy.Id = urlSplit[4];
                handy.Url = url;
                handy.Model = name.InnerText.Replace(" Daten", "").Trim();
                handy.ImageUrl = imageSplit[3].Split('?')[0];
                handy.Display = display;
                handy.Casing = casing;
                handy.Hardware= hardware;
                handy.Connectivity= connectivity;
                handy.ConnectionTransmission = connectTrans;
                handy.Cameras = cameras;
                handy.Miscellaneous = misc;
                return handy;
            }
            return new();
        }

    }
}
