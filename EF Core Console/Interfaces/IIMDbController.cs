namespace EF_Core_Console.Interfaces
{
    public interface IIMDbController
    {
        Task GetMovie(string url);
        Task LoadTop250();
    }
}