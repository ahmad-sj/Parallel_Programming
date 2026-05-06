using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Qty { get; set; }

        // Navigational
        public Product Product { get; set; } = null!;
        public Cart Cart { get; set; } = null!;
        public Guid CartId { get; set; }
    }
}
