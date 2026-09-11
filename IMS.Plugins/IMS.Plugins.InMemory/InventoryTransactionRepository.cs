using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly IInventoryRepository inventoryRepository;
        public List<InventoryTransaction> _inventoryTransactions = new List<InventoryTransaction>();

        public InventoryTransactionRepository(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? startDate, DateTime? endDate, InventoryTransactionType? transactionType)
        {
            var inventories = (await inventoryRepository.GetInventoriesByNameAsync(string.Empty)).ToList();
            var query = from it in _inventoryTransactions
                        join inv in inventories on it.InventoryId equals inv.InventoryId
                        where (string.IsNullOrWhiteSpace(inventoryName) || inv.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0)
                        && (!startDate.HasValue || it.TransactionDate >= startDate.Value.Date)
                        && (!endDate.HasValue || it.TransactionDate <= endDate.Value.Date)
                        && (!transactionType.HasValue || it.ActivityType == transactionType.Value)
                        select new InventoryTransaction
                        {
                            Inventory = inv,
                            InventoryTransactionId = it.InventoryTransactionId,
                            PurchaseOrderNumber = it.PurchaseOrderNumber,
                            ProductionNumber = it.ProductionNumber,
                            InventoryId = it.InventoryId,
                            QuantityBefore = it.QuantityBefore,
                            ActivityType = it.ActivityType,
                            QuantityAfter = it.QuantityAfter,
                            UnitPrice = it.UnitPrice,
                            TransactionDate = it.TransactionDate,
                            DoneBy = it.DoneBy
						};
            return query.ToList();
		}

        public void ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            _inventoryTransactions.Add(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.ProduceProduct,
                QuantityAfter = inventory.Quantity - quantityToConsume,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });
        }

        public void PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            _inventoryTransactions.Add(new InventoryTransaction 
            {
                PurchaseOrderNumber = poNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                QuantityAfter = inventory.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });
        }
    }
}
