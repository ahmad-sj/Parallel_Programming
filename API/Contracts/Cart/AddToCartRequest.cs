namespace API.Contracts.Cart
{
    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
    }
}
