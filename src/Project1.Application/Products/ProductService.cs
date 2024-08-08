using Project1.Core.Generals.Interfaces;
using Project1.Core.Products.Entities;
using Project1.Core.Products.Interfaces;
using Project1.Core.Products.Interfaces.DTOs;

namespace Project1.Application.Products;

public class ProductService(IGenericRepository<Product> productRepo) : IProductService
{
    public async Task<long> AddProduct(AddProductDTO input)
    {
        var product = new Product { Name = input.Name };
        await productRepo.AddEntity(product);
        await productRepo.SaveChanges();
        return product.Id;
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
