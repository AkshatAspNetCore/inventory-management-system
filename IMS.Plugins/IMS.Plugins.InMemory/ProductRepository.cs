using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> _products;

        public ProductRepository() 
        {
            _products = new List<Product>() 
            {
                new Product{ ProductId = 1, ProductName = "Bike", Quantity = 10, Price = 500},
                new Product{ ProductId = 2, ProductName = "Car", Quantity = 10, Price = 15000},
            };
        }

        public Task AddProductAsync(Product Product)
        {
            if(_products.Any(x => x.ProductName.Equals(Product.ProductName, StringComparison.OrdinalIgnoreCase))) 
            {
                return Task.CompletedTask;
            }

            var maxId = _products.Max(x => x.ProductId);
            Product.ProductId = maxId + 1;

            _products.Add(Product);
            return Task.CompletedTask;
        }

        public Task DeleteProductAsync(int ProductId)
        {
           var ProductToDelete = _products.FirstOrDefault(x => x.ProductId == ProductId);
            if (ProductToDelete != null)
                _products.Remove(ProductToDelete);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) return await Task.FromResult(_products);

            return _products.Where(x => x.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Product?> GetProductByIdAsync(int ProductId)
        {
            var product = _products.FirstOrDefault(x => x.ProductId == ProductId);
            var newProduct = new Product();

            if (product != null)
            {
                newProduct.ProductId = product?.ProductId ?? 0;
                newProduct.ProductName = product?.ProductName ?? string.Empty;
                newProduct.Quantity = product?.Quantity ?? 0;
                newProduct.Price = product?.Price ?? 0;
                newProduct.ProductInventories = new List<ProductInventory>();

                if (product.ProductInventories != null && product.ProductInventories.Count > 0)
                {
                    foreach (var productInventory in product.ProductInventories)
                    {
                        var newProductInventory = new ProductInventory
                        {
                            ProductId = productInventory.ProductId,
                            Product = newProduct,
                            InventoryId = productInventory.InventoryId,
                            Inventory = new Inventory(),
                            InventoryQuantity = productInventory.InventoryQuantity
                        };

                        if (productInventory.Inventory != null)
                        {
                            newProductInventory.Inventory.InventoryId = productInventory.Inventory.InventoryId;
                            newProductInventory.Inventory.InventoryName = productInventory.Inventory.InventoryName;
                            newProductInventory.Inventory.Quantity = productInventory.Inventory.Quantity;
                            newProductInventory.Inventory.Price = productInventory.Inventory.Price;
                        }

                        newProduct.ProductInventories.Add(newProductInventory);
                    }
                }
            }
            return await Task.FromResult<Product?>(newProduct);
        }

        public Task UpdateProductAsync(Product Product)
        {
            if(_products.Any(x => x.ProductId != Product.ProductId && x.ProductName.Equals(Product.ProductName, StringComparison.OrdinalIgnoreCase)))
                return Task.CompletedTask;

            var productToUpdate = _products.FirstOrDefault(x => x.ProductId == Product.ProductId);
            if (productToUpdate != null)
            {
                productToUpdate.ProductName = Product.ProductName;
                productToUpdate.Quantity = Product.Quantity;
                productToUpdate.Price = Product.Price;
                productToUpdate.ProductInventories = Product.ProductInventories;
            }

            return Task.CompletedTask;
        }
    }
}
