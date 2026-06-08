using Application.Services;
using Application.Usecases.System.Emails;
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


namespace Application.Usecases.Users.Carts.Checkout;

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
        Helpers.PrintTimestamp("========================= Before Checkout =========================");
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand command)
    {
        // start transaction
        await using var transaction = await _repository.BeginTransactionAsync();
        Helpers.PrintTimestamp("Transaction Started");

        int orderSeqId = 0;

        try
        {
            Helpers.PrintTimestamp("Checking out ...");

            // get cart
            var cart = await _repository.GetAsync<Cart>(x => x.UserId == command.UserId, opt => opt.Include(i => i.Items).ThenInclude(i => i.Product));

            if (cart == null || !cart.Items.Any())
            {
                throw new Exception("Cart is empty!");
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

            orderSeqId = order.SeqId;

            // clear items & remove cart
            cart.Items.Clear();
            _repository.Remove(cart);

            Helpers.PrintTimestamp($"Order '{order.SeqId}' Created and Cart is Cleared");
            await _repository.SaveChangesAsync();

            // process payment
            await PaymentService.ProcessPayment(command.PaymentSuccess);

            // send order confirmation by email
            await _messageBus.PublishAsync<OrderConfirmationCommand>(OrderConfirmationCommand.Create(command.UserId, order.Id));
            Helpers.PrintTimestamp("Order Confirmation Message is Published");

            // commit transaction
            await transaction.CommitAsync();
            Helpers.PrintTimestamp($"Transaction Commited for Order '{orderSeqId}'");

            return new CheckoutResult(order.Id);
        }
        catch (Exception ex)
        {
            // rollback transaction
            await transaction.RollbackAsync();
            Helpers.PrintTimestamp($"Transaction Rolled Back for Order '{orderSeqId}'");

            Console.WriteLine(ex.Message);

            throw new Exception(ex.Message);
        }
    }

    public async Task Finally(CheckoutCommand command)
    {
        Helpers.PrintTimestamp("========================= After Checkout ==========================");
    }


}


