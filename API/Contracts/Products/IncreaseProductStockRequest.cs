namespace API.Contracts.Products;

public class IncreaseProductStockRequest
{
    public int ProductId { get; set; }
    public int IncreaseQty { get; set; }
}
