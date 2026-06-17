using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.System.Emails;

public class OrderConfirmationCommand
{
    public int UserId { get; set; }
    public int OrderId { get; set; }

    public static OrderConfirmationCommand Create(int userId, int orderId)
    {
        return new OrderConfirmationCommand { UserId = userId, OrderId = orderId };
    }
}
