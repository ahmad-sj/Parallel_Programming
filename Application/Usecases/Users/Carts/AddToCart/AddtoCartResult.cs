namespace Application.Usecases.Users.Carts.AddToCart
{
    public class AddtoCartResult
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<AddtoCartItemResult> Items { get; set; } = [];
    }

    public class AddtoCartItemResult
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
}
