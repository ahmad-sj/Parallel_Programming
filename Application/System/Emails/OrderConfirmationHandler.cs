using Application.Users.Carts.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Emails;

public class OrderConfirmationHandler
{
    public async Task Before(CheckoutCommand command)
    {
        string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine("\n" + timeWithMS + "\t================== Before Order Confirmation =================");
    }

    public async Task Handle(OrderConfirmationCommand command)
    {
        string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine("\n" + timeWithMS + "\tSending Confirmation...");
        await Task.Delay(10_000);

        timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine("\n" + timeWithMS + "\tConfirmation Sent ✓");
    }

    public async Task After(CheckoutCommand command)
    {
        string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine("\n" + timeWithMS + "\t================== After Order Confirmation ==================");
    }
}
