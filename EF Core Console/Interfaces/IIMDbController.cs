namespace EF_Core_Console.Interfaces
{
    public interface IIMDbController
    {
        Task<Movie> GetMovie(string url);
        Task LoadTop250();
    }
}