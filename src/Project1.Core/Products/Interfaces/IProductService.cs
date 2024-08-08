using Project1.Core.Products.Interfaces.DTOs;

namespace Project1.Core.Products.Interfaces;

public interface IProductService
{
    Task<long> AddProduct(AddProductDTO input);
}
