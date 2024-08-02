using Project1.Core.General.Interfaces;
using Project1.Core.ProductAggregate.Entities;
using Project1.Core.ProductAggregate.Interfaces;
using Project1.Core.ProductAggregate.Interfaces.DTOs;

namespace Project1.Application.ProductServices;

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
}
