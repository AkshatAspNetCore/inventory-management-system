using IMS.CoreBusiness;
using IMS.Plugins.EFCoreSqlServer;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionEFCoreRepository : IProductTransactionRepository
    {
        private readonly IDbContextFactory<IMSContext> contextFactory;
        private readonly IProductRepository productRepository;
        private readonly IInventoryTransactionRepository inventoryTransactionRepository;
        private readonly IInventoryRepository inventoryRepository;

        public ProductTransactionEFCoreRepository(IDbContextFactory<IMSContext> contextFactory, IProductRepository productRepository, IInventoryTransactionRepository inventoryTransactionRepository, IInventoryRepository inventoryRepository)
        {
            this.contextFactory = contextFactory;
            this.productRepository = productRepository;
            this.inventoryTransactionRepository = inventoryTransactionRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            using var db = contextFactory.CreateDbContext();

            var prod = await productRepository.GetProductByIdAsync(product.ProductId);
            if (prod is not null)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    // add inventory transaction record
                    if (pi.Inventory is not null)
                    {
                        await inventoryTransactionRepository.ProduceAsync(productionNumber, pi.Inventory, pi.InventoryQuantity * quantity, doneBy, -1);
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
                db.Add(new ProductTransaction
                {
                    ProductionNumber = productionNumber,
                    ProductId = product.ProductId,
                    QuantityBefore = product.Quantity,
                    ActivityType = ProductTransactionType.ProduceProduct,
                    QuantityAfter = product.Quantity + quantity,
                    DoneBy = doneBy,
                    TransactionDate = DateTime.Now,
                    UnitPrice = 0
                });

                await db.SaveChangesAsync();
            }
        }

        public async Task SellProductAsync(string saleOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            using var db = contextFactory.CreateDbContext();
            db.Add(new ProductTransaction
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

            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string productName, DateTime? startDate, DateTime? endDate, ProductTransactionType? transactionType)
        {
            using var db = contextFactory.CreateDbContext();
            var query = from it in db.ProductTransactions
                        join inv in db.Products on it.ProductId equals inv.ProductId
                        where (string.IsNullOrWhiteSpace(productName) || inv.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0)
                        && (!startDate.HasValue || it.TransactionDate >= startDate.Value.Date)
                        && (!endDate.HasValue || it.TransactionDate <= endDate.Value.Date)
                        && (!transactionType.HasValue || it.ActivityType == transactionType.Value)
                        select it;

            return await query.Include(x => x.Product).ToListAsync();
        }
    }
}
