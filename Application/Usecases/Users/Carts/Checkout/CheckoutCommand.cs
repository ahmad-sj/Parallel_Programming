using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.Users.Carts.Checkout;

public class CheckoutCommand //: ILockableCommand
{
    public int UserId { get; set; }
    public bool PaymentSuccess { get; set; }
    public int PaymentTime { get; set; }

    public string LockKey => $"checkout:user:{UserId}";
}
