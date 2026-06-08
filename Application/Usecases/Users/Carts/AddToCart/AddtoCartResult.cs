namespace Application.Usecases.Users.Carts.AddToCart
{
    public class AddtoCartResult
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<AddtoCartItemResult> Items { get; set; } = [];
    }

    public class AddtoCartItemResult
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
}
