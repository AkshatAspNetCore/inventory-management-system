using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new List<ProductTransaction>();

        private readonly IProductRepository productRepository;
        private readonly IInventoryTransactionRepository inventoryTransactionRepository;
        private readonly IInventoryRepository inventoryRepository;

        public ProductTransactionRepository(IProductRepository productRepository, IInventoryTransactionRepository inventoryTransactionRepository, IInventoryRepository inventoryRepository)
        {
            this.productRepository = productRepository;
            this.inventoryTransactionRepository = inventoryTransactionRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            var prod = await productRepository.GetProductByIdAsync(product.ProductId);
            if (prod is not null)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    // add inventory transaction record
                    if (pi.Inventory is not null)
                    {
                        inventoryTransactionRepository.ProduceAsync(productionNumber, pi.Inventory, pi.InventoryQuantity * quantity, doneBy, -1);
                    }


                    // decrease the inventory quantity
                    var inv = await inventoryRepository.GetInventoryByIdAsync(pi.InventoryId);
                    if (inv is not null)
                    {
                        inv.Quantity -= pi.InventoryQuantity * quantity;
                        await inventoryRepository.UpdateInventoryAsync(inv);
                    }
                }

                // add product transaction record
                _productTransactions.Add(new ProductTransaction
                {
                    ProductionNumber = productionNumber,
                    ProductId = product.ProductId,
                    QuantityBefore = product.Quantity,
                    ActivityType = ProductTransactionType.ProduceProduct,
                    QuantityAfter = product.Quantity + quantity,
                    DoneBy = doneBy,
                    TransactionDate = DateTime.Now
                });
            }
        }

        public Task SellProductAsync(string saleOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            _productTransactions.Add(new ProductTransaction
            {
                SaleOrderNumber = saleOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.SellProduct,
                QuantityAfter = product.Quantity - quantity,
                DoneBy = doneBy,
                TransactionDate = DateTime.Now,
                UnitPrice = unitPrice
            });

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string productName, DateTime? startDate, DateTime? endDate, ProductTransactionType? transactionType)
        {
            var products = (await productRepository.GetProductsByNameAsync(string.Empty)).ToList();
            var query = from it in _productTransactions
                        join inv in products on it.ProductId equals inv.ProductId
                        where (string.IsNullOrWhiteSpace(productName) || inv.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0)
                        && (!startDate.HasValue || it.TransactionDate >= startDate.Value.Date)
                        && (!endDate.HasValue || it.TransactionDate <= endDate.Value.Date)
                        && (!transactionType.HasValue || it.ActivityType == transactionType.Value)
                        select new ProductTransaction
                        {
                            Product = inv,
                            ProductTransactionId = it.ProductTransactionId,
                            SaleOrderNumber = it.SaleOrderNumber,
                            ProductionNumber = it.ProductionNumber,
                            ProductId = it.ProductId,
                            QuantityBefore = it.QuantityBefore,
                            ActivityType = it.ActivityType,
                            QuantityAfter = it.QuantityAfter,
                            UnitPrice = it.UnitPrice,
                            TransactionDate = it.TransactionDate,
                            DoneBy = it.DoneBy
                        };
            return query.ToList();
        }
    }
}
