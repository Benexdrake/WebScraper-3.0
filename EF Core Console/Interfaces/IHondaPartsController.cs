namespace EF_Core_Console.Interfaces
{
    public interface IHondaPartsController
    {
        Task FullUpdate(bool relatedparts, string url);
        Task GetPart(CategoryUrl url);
    }
}