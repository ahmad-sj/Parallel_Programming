using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Qty { get; set; }

        // Navigational
        public Order Order { get; set; } = null!;
        public Guid OrderId { get; set; }
    }
}
