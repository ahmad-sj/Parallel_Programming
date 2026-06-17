using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public static class PaymentService
{
    public static async Task ProcessPayment(bool success, int time)
    {
        if (success)
        {
            Helpers.PrintTimestamp("Processing payment ...");

            await Task.Delay(time);

            Helpers.PrintTimestamp("Payment Succeeded ✓");
        }
        else
        {
            Helpers.PrintTimestamp("Processing payment ...");

            await Task.Delay(time);

            Helpers.PrintTimestamp("Payment Failed ✗");

            throw new Exception("Payment failed Exception");
        }
    }
}
