namespace Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ICollection<CartItem> Items { get; set; } = [];

        public static Cart Create(Guid userId)
        {
            return new Cart
            {
                UserId = userId,
            };
        }
    }
}
