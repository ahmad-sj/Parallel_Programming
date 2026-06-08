using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ICollection<CartItem> Items { get; set; } = [];

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeqId { get; set; }

        public static Cart Create(Guid userId)
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
