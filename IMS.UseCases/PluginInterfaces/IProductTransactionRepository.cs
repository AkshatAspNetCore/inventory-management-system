using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductTransactionRepository
    {
        Task ProduceAsync(string productionNnumber, Product product, int quantity, string doneBy);
        Task SellProductAsync(string saleOrderNumber, Product product, int quantity, double unitPrice, string doneBy);
    }
}