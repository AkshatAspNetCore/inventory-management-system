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

        public Task<Product?> GetProductByIdAsync(int ProductId)
        {
            return Task.FromResult<Product?>(_products.FirstOrDefault(x => x.ProductId == ProductId));
        }

        public Task UpdateProductAsync(Product Product)
        {
            if(_products.Any(x => x.ProductId != Product.ProductId && x.ProductName.Equals(Product.ProductName, StringComparison.OrdinalIgnoreCase)))
                return Task.CompletedTask;

            var invToUpdate = _products.FirstOrDefault(x => x.ProductId == Product.ProductId);
            if (invToUpdate != null)
            {
                invToUpdate.ProductName = Product.ProductName;
                invToUpdate.Quantity = Product.Quantity;
                invToUpdate.Price = Product.Price;
            }

            return Task.CompletedTask;
        }
    }
}
