using Project1.Core.Generals.Interfaces;
using Project1.Core.Products.Entities;
using Project1.Core.Products.Interfaces;
using Project1.Core.Products.Interfaces.DTOs;

namespace Project1.Application.Products;

public class ProductService(IGenericRepository<Product> productRepo, ICacheManager cacheManager) : IProductService
{
    public async Task<long> AddProduct(AddProductDTO input)
    {
        var product = new Product { Name = input.Name };
        await productRepo.AddEntity(product);
        await productRepo.SaveChanges();
        return product.Id;
    }

    public async Task<Product> GetProductAsync(long productId)
    {
        string cacheKey = $"Product_{productId}";
        var product = cacheManager.Get<Product>(cacheKey);

        if (product == null)
        {
            product = await productRepo.GetEntityById(productId);
            if (product != null)
            {
                cacheManager.Set(cacheKey, product, TimeSpan.FromHours(1));
            }
        }

        return product;
    }

    public async ValueTask DisposeAsync()
    {
        await productRepo.DisposeAsync();
    }

    public void Dispose()
    {
        productRepo.Dispose();
    }
}
