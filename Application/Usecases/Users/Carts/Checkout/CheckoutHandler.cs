using Application.Services;
using Application.Usecases.System.Emails;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TanvirArjel.EFCore.GenericRepository;
using Wolverine;

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
        var t0 = Stopwatch.GetTimestamp();

        // Start Transaction
        await using var transaction = await _repository.BeginTransactionAsync();
        Helpers.PrintTimestamp($"Transaction Started");

        int orderId = 0;

        try
        {
            //Getting Cart, Creating Order, Reducing Stock, Clearing Cart, Processing Payment
            #region
            Helpers.PrintTimestamp("Checking out ...");

            // Get Cart
            var cart = await _repository.GetAsync<Cart>(x => x.UserId == command.UserId, opt => opt.Include(i => i.Items).ThenInclude(i => i.Product));

            var t1 = Stopwatch.GetTimestamp();
            Helpers.PrintTimestamp($"[PERF] Cart Read took: {Stopwatch.GetElapsedTime(t0, t1).TotalMilliseconds} ms");

            if (cart == null || !cart.Items.Any())
                throw new Exception("Cart is empty!");

            // Prepare Order Items
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
                Helpers.PrintTimestamp($"Product '{item.Product.Id}' stock qty '{item.Product.Qty}', deducting '{item.Qty}' ");
                // Reduce Stock Quantity
                item.Product.Qty -= item.Qty;
            }

            // Create order
            var order = Order.Create(command.UserId, orderItems, PaymentMethodType.Card);
            await _repository.AddAsync<Order>(order);

            // Clear Cart Items & Remove it
            cart.Items.Clear();
            _repository.Remove(cart);

            await _repository.SaveChangesAsync();

            var t2 = Stopwatch.GetTimestamp();
            Helpers.PrintTimestamp($"[PERF] Order Create + SaveChanges took: {Stopwatch.GetElapsedTime(t1, t2).TotalMilliseconds} ms");

            orderId = order.Id;
            Helpers.PrintTimestamp($"Order '{orderId}' Created and Cart is Cleared");

            // Process Payment
            await PaymentService.ProcessPayment(command.PaymentSuccess, command.PaymentTime);

            var t3 = Stopwatch.GetTimestamp();
            Helpers.PrintTimestamp($"[PERF] Payment Processing took: {Stopwatch.GetElapsedTime(t2, t3).TotalMilliseconds} ms");
            #endregion

            // Order Confirmation
            #region
            // Send confirmation using direct service call (synchronous)
            // await ConfirmationService.SendEmail(command.UserId, order.SeqId);

            var t4 = Stopwatch.GetTimestamp();
            // Send confirmation using message bus (asynchronous)
            await _messageBus.PublishAsync<OrderConfirmationCommand>(
                OrderConfirmationCommand.Create(command.UserId, order.Id));
            var t5 = Stopwatch.GetTimestamp();

            Helpers.PrintTimestamp($"[PERF] Published confirmation command to message bus took: " +
                                   $"{Stopwatch.GetElapsedTime(t4, t5).TotalMilliseconds} ms");
            #endregion

            // Commit Transaction
            await transaction.CommitAsync();
            Helpers.PrintTimestamp($"[PERF] Transaction Committed for Order '{orderId}'");

            Helpers.PrintTimestamp($"[PERF] Transaction Committed for Order '{orderId}', total time is: " +
                                   $"{Stopwatch.GetElapsedTime(t0, t4).TotalMilliseconds} ms");

            return new CheckoutResult { Message = "Order has been place successfully", OrderId = order.Id, Success = true };
        }
        catch (Exception ex)
        {
            // Rollback Transaction
            await transaction.RollbackAsync();
            Helpers.PrintTimestamp($"Transaction Rolled Back for Order '{orderId}'");
            Helpers.PrintTimestamp($"Order placing failed, please try again later");

            if (ex is DbUpdateConcurrencyException)
                Helpers.PrintTimestamp($"Multiple users are trying to update the same product stock");

            return new CheckoutResult { Message = "Order placing failed", OrderId = -1, Success = false };
        }
    }

    public async Task Finally(CheckoutCommand command)
    {
        Helpers.PrintTimestamp("========================= After Checkout ==========================");
    }
}


