using Application.Services;
using Application.Usecases.Users.Carts.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Usecases.System.Emails;

public class OrderConfirmationHandler
{
    public async Task Handle(OrderConfirmationCommand command)
    {
        await ConfirmationService.SendEmail(command.UserId, command.OrderId);
    }
}
