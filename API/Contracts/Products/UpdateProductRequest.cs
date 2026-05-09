namespace API.Contracts.Products;

public class UpdateProductRequest
{
    public Guid ProductId { get; set; }
    public int ProductQty { get; set; }
}
