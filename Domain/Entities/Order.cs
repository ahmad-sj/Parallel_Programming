using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public ICollection<OrderItem> Items { get; set; } = null!;
        public DateTime Date { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public bool Paid { get; set; }
        public Guid UsedrId { get; set; }

        public static Order Create(Guid userId, List<OrderItem> orderItems, PaymentMethodType paymentMethod)
        {
            var order = new Order
            {
                Items = orderItems,
                Date = DateTime.Now,
                PaymentMethod = paymentMethod,
                Paid = true,
                UsedrId = userId
            };

            return order;
        }
    }

    public enum PaymentMethodType
    {
        COD = 1, // Cash On Delivery
        Card = 2
    }

    public enum OrderStatus
    {
        Pending = 1,
        Complete = 2,
        Canceled = 3,
        Declined = 4,

    }
}
