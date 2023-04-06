namespace Webscraper.Models.TVProgramm.Models;

public class TV
{
    public string Sender { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public string Sparte { get; set; }
    public TimeOnly Zeit_Von { get; set; }
    public TimeOnly Zeit_Bis { get; set; }
    public DateOnly Datum { get; set; }
    public string Tag { get; set; }
}
