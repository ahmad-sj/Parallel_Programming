namespace API.Contracts.Cart
{
    public class AddToCartRequest
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
}
