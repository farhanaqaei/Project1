using FastEndpoints;
using Project1.Core.Products.Interfaces;
using Project1.Core.Products.Interfaces.DTOs;

namespace Project1.API.ProductEndPoints;

public class ProductEndPoint(IProductService productService) : Endpoint<AddProductDTO, long>
{
    public override void Configure()
    {
        Post("/api/product");
        AllowAnonymous();
    }
    public override async Task HandleAsync(AddProductDTO req, CancellationToken ct)
    {
        long productId = await productService.AddProduct(req);

        await SendAsync(productId, cancellation: ct);
    }
}
