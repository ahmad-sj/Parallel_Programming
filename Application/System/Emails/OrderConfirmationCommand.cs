using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Emails;

public class OrderConfirmationCommand
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }

    public static OrderConfirmationCommand Create(Guid userId, Guid orderId)
    {
        return new OrderConfirmationCommand { UserId = userId, OrderId = orderId };
    }
}
