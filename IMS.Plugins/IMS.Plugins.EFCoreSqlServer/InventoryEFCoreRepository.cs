using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private readonly IDbContextFactory<IMSContext> contextFactory;

        public InventoryEFCoreRepository(IDbContextFactory<IMSContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            using var db = contextFactory.CreateDbContext();
            db.Inventories?.Add(inventory);
            await db.SaveChangesAsync();
        }

        public async Task DeleteInventoryAsync(int inventoryId)
        {
            using var db = contextFactory.CreateDbContext();
            var inventory = db.Inventories?.Find(inventoryId);
            if (inventory is null) return;

            db.Inventories?.Remove(inventory);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            using var db = contextFactory.CreateDbContext();

            return await db.Inventories.Where(x => x.InventoryName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();
        }

        public async Task<Inventory?> GetInventoryByIdAsync(int inventoryId)
        {
            using var db = contextFactory.CreateDbContext();
            var inventory = await db.Inventories.FindAsync(inventoryId);
            
            return inventory;

        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            using var db = contextFactory.CreateDbContext();
            var inv = await db.Inventories.FindAsync(inventory.InventoryId);
            if (inv is not null) 
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Quantity = inventory.Quantity;
                inv.Price = inventory.Price;
            }

            await db.SaveChangesAsync();
        }
    }
}
