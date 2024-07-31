using Project1.Core.ProductAggregate.Interfaces.DTOs;

namespace Project1.Core.ProductAggregate.Interfaces;

public interface IProductService
{
    long AddProduct(AddProductDTO input);
}
