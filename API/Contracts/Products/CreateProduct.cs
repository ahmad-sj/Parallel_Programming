namespace API.Contracts.Products
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty!;
        public double Price { get; set; }
        public int Qty { get; set; }
    }
}
