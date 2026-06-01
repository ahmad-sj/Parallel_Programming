using Application.Admin.Products.AddProduct;

namespace Application.Admin.Products.AddProducts;

public class AddProductsCommand
{
    public List<AddProductCommand> List { get; set; } = null!;
}
