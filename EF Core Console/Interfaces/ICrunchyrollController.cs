namespace EF_Core_Console.Interfaces
{
    public interface ICrunchyrollController
    {
        Task Debug(string url);
        Task FullUpdateAnimes();
    }
}