using Project1.Core.ProductAggregate.Entities;
using Project1.Core.ProductAggregate.Interfaces;
using Project1.Core.ProductAggregate.Interfaces.DTOs;
using Project1.Infrastructure.Data;

namespace Project1.Application.ProductServices;

public class ProductService(ApplicationDbContext context) : IProductService
{
    public long AddProduct(AddProductDTO input)
    {
        var product = new Product { Name = input.Name };
        context.Add(product);
        context.SaveChanges();
        return product.Id;
    }
}
