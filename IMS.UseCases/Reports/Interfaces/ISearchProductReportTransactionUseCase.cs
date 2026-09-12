using IMS.CoreBusiness;

namespace IMS.UseCases.Reports.Interfaces
{
    public interface ISearchProductReportTransactionUseCase
    {
        Task<IEnumerable<ProductTransaction>> ExecuteAsync(string productName, DateTime? startDate, DateTime? endDate, ProductTransactionType? transactionType);
    }
}