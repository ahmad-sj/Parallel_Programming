using Domain.Entities;

namespace API.Contracts.Products;

public class AddProductsRequest
{
    public List<AddProductRequest> List { get; set; } = []!;
}
