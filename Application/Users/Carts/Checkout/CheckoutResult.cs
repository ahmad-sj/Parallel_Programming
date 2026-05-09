using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Carts.Checkout;

public class CheckoutResult
{
    public Guid OrderId { get; set; }
    public string msg { get; set; }

    public CheckoutResult(Guid orderId)
    {
        OrderId = orderId;
        msg = "Order has been place successfully";
    }
}
