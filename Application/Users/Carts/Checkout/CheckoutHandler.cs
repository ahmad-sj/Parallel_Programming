using Application.System.Emails;
using Application.Users.Auth;
using Application.Users.Carts.AddToCart;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;
using Wolverine;
using Wolverine.Shims.MediatR;

namespace Application.Users.Carts.Checkout;

public class CheckoutHandler
{
    private readonly IRepository _repository;
    private readonly IMessageBus _messageBus;

    public CheckoutHandler(IRepository repository, IMessageBus messageBus)
    {
        _repository = repository;
        _messageBus = messageBus;
    }

    public async Task Before(CheckoutCommand command)
    {
        string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine("\n" + timeWithMS + "\t======================= Before Checkout =======================");
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand command)
    {
        try
        {
            string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine("\n" + timeWithMS + "\tChecking Out Started");

            // get cart
            var cart = await _repository.GetAsync<Cart>(x => x.UserId == command.UserId, opt => opt.Include(i => i.Items).ThenInclude(i => i.Product));
            if (cart == null || !cart.Items.Any())
            {
                throw new Exception("Cart is empty");
            }

            // prepare order items
            var orderItems = new List<OrderItem>();

            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Name = item.ProductName,
                    Price = item.Product.Price,
                    Qty = item.Qty
                };
                orderItems.Add(orderItem);
            }

            // create order
            var order = Order.Create(command.UserId, orderItems, PaymentMethodType.Card);
            await _repository.AddAsync<Order>(order);

            // clear items & remove cart
            cart.Items.Clear();
            _repository.Remove(cart);

            await _repository.SaveChangesAsync();

            // send order confirmation by email
            await _messageBus.PublishAsync<OrderConfirmationCommand>(OrderConfirmationCommand.Create(command.UserId, order.Id));

            timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine("\n" + timeWithMS + "\tMessage Published");
            

            return new CheckoutResult(order.Id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task After(CheckoutCommand command)
    {
        string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine("\n" + timeWithMS + "\t======================= After Checkout =======================");
    }
}
