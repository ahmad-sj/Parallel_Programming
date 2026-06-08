using Application.Usecases.Admin.Products.AddProduct;

namespace Application.Usecases.Admin.Products.AddProducts;

public class AddProductsCommand
{
    public List<AddProductCommand> List { get; set; } = null!;
}
