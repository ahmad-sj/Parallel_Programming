namespace API.Contracts.Cart
{
    public class CheckoutRequest
    {
        public Guid UserId { get; set; }
        public bool PaymentSuccess { get; set; }
    }
}
