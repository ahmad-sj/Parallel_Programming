using Application.Usecases.Users.Carts.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.System.Emails;

public class OrderConfirmationHandler
{
    public async Task Before(OrderConfirmationCommand command)
    {
        //Helpers.PrintTimestamp("==================== Before Order Confirmation ====================");
    }

    public async Task Handle(OrderConfirmationCommand command)
    {
        Helpers.PrintTimestamp("Sending Confirmation...");

        await Task.Delay(4000);

        Helpers.PrintTimestamp("Order Confirmation Sent Successfully ✓");
    }

    public async Task After(OrderConfirmationCommand command)
    {
        //Helpers.PrintTimestamp("==================== After Order Confirmation =====================");
    }
}
