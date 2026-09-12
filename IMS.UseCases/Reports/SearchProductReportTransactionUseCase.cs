using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Reports
{
    public class SearchProductReportTransactionUseCase : ISearchProductReportTransactionUseCase
    {
        private readonly IProductTransactionRepository productTransactionRepository;

        public SearchProductReportTransactionUseCase(IProductTransactionRepository productTransactionRepository)
        {
            this.productTransactionRepository = productTransactionRepository;
        }

        public async Task<IEnumerable<ProductTransaction>> ExecuteAsync(string productName, DateTime? startDate, DateTime? endDate, ProductTransactionType? transactionType)
        {
            if(endDate.HasValue) endDate = endDate.Value.AddDays(1).AddTicks(-1); // Set to the end of the day

            return await productTransactionRepository.GetProductTransactionsAsync(
                productName,
                startDate,
                endDate,
                transactionType);
        }
    }
}
