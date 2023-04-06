namespace Webscraper.API.Scraper.TVProgramm.Controllers;

public class TV_API
{
    private readonly Browser _browser;
    public TV_API(Browser browser)
    {
        _browser = browser;
    }

    public async Task<TV[]> GetAllTVProgramms()
    {
        var tvPrograms = new List<TV>();
        int i = 1;
        while (true)
        {
            string url = $"https://www.tvspielfilm.de/tv-programm/sparte/?page={i}&filter=1&order=channel&date=thisWeek&freetv=1&cat[0]=SP&cat[1]=SE&time=day&channel=";
            var doc = _browser.GetPageDocument(url, 1000).Result;
            var main = Helper.FindNodesByDocument(doc, "article", "class", "program-section tvlistings").Result.FirstOrDefault();
            if(main is null)
            {
                break;
            }
            var page = await GetTVPage(main);
            tvPrograms.AddRange(page);
            i++;
        }
       return tvPrograms.ToArray();
    }

    private async Task<TV[]> GetTVPage(HtmlNode main)
    {
        var tvs = new List<TV>();
        var table = Helper.FindNodesByNode(main, "table", "class", "info-table").Result.FirstOrDefault();
        if (table is not null) 
        {
            var trs = Helper.FindNodesByNode(table, "tr", "class", "hover").Result;
            foreach ( var tr in trs ) 
            {
                var tv = await GetTVProgram(tr);
                if (tv is not null)
                tvs.Add(tv);
            }
        }
        return tvs.ToArray();
    }

    private async Task<TV> GetTVProgram(HtmlNode row)
    {
        var tv = new TV();
        var cols = Helper.FindNodesByNode(row, "td", "class", "col").Result;

        // Sender
        var sender = Helper.FindNodesByNode(cols[0],"img","src", "https://a2.tvspielfilm.de/images/tv/sender/mini/").Result.FirstOrDefault();
        var senderSplit = sender.OuterHtml.Split('"');
        tv.Sender = senderSplit[3].Replace(" Programm","");

        // Zeit Strong
        var time = Helper.FindNodesByNode(cols[1], "strong", "", "").Result.FirstOrDefault();
        var timeSplit = time.InnerText.Split("-");
            // Zeit von
            tv.Zeit_Von = TimeOnly.Parse(timeSplit[0].Trim());

            // Zeit bis
            tv.Zeit_Bis = TimeOnly.Parse(timeSplit[1].Trim());
        // Datum Tag Span
        var date = Helper.FindNodesByNode(cols[1], "span", "", "").Result.FirstOrDefault();
        var dateSplit = date.InnerText.Split(" ");
        // Datum
        tv.Datum = DateOnly.Parse(dateSplit[1].Trim() + DateTime.Now.Year);
        // Tag 1
        tv.Tag = dateSplit[0].Trim();
        // Titel 2
        var title = Helper.FindNodesByNode(cols[2],"strong","","").Result.FirstOrDefault();
        tv.Title = title.InnerText;
        // Genre 3
        tv.Genre = cols[3].InnerText.Trim();
        // Sparte 4
        var split = cols[4].InnerText.Split(" ");

        tv.Sparte = split.Where(x => x.Length > 4).FirstOrDefault().Trim();
        return tv;
    }

}
