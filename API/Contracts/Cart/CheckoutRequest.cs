namespace API.Contracts.Cart
{
    public class CheckoutRequest
    {
        public int UserId { get; set; }
        public bool PaymentSuccess { get; set; }

        public int PaymentTime { get; set; }
    }
}
