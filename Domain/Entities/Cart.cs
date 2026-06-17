using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItem> Items { get; set; } = [];

        public static Cart Create(int userId)
        {
            return new Cart
            {
                UserId = userId,
            };
        }

        public void AddItem(CartItem cartItem)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Qty += cartItem.Qty;
            }
            else
            {
                Items.Add(cartItem);
            }
        }
    }
}
