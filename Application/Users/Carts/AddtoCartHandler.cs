// ============================================================
// الملف: Application/Users/Carts/AddtoCartHandler.cs (مُصلَح)
// ============================================================

using Application.Notifications;
using Application.Services;   // ← هذا السطر فقط تغيّر (مش API.Services)
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Users.Carts;

public class AddtoCartHandler
{
    private readonly IRepository _repository;
    private readonly OrderNotificationQueue _notificationQueue;

    public AddtoCartHandler(
        IRepository repository,
        OrderNotificationQueue notificationQueue)
    {
        _repository = repository;
        _notificationQueue = notificationQueue;
    }

    public async Task<AddtoCartResult> Handle(AddtoCartCommand command)
    {
        try
        {
            var product = await _repository.GetQueryable<Product>()
                .Where(x => x.Id == command.ProductId)
                .FirstOrDefaultAsync();

            if (product == null)
                throw new Exception("Product not found exception");

            if (command.Qty > product.Qty)
                throw new Exception("Product stock is not sufficient");

            product.Qty -= command.Qty;

            var cart = await _repository.GetAsync<Cart>(x => x.UserId == command.UserId);
            var isNewCart = cart == null;

            if (isNewCart)
            {
                cart = Cart.Create(command.UserId);
                await _repository.AddAsync<Cart>(cart);
            }

            cart!.Items.Add(new CartItem
            {
                ProductId = command.ProductId,
                ProductName = command.ProductName,
                Qty = command.Qty,
            });

            if (!isNewCart) _repository.Update(cart);
            _repository.Update(product);
            await _repository.SaveChangesAsync();

            // ✅ المتطلب 3: Queue بدون انتظار
            await _notificationQueue.EnqueueAsync(new SendOrderNotificationCommand
            {
                UserId = command.UserId,
                ProductName = command.ProductName,
                Qty = command.Qty
            });

            return new AddtoCartResult
            {
                Id = command.ProductId,
                UserId = command.UserId,
                Items = cart.Items.Select(x => new AddtoCartItemResult
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    ProductId = x.ProductId,
                    Qty = x.Qty,
                }).ToList()
            };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            var dbValues = await entry.GetDatabaseValuesAsync();
            if (dbValues == null) throw new Exception("Entity was deleted");
            entry.OriginalValues.SetValues(dbValues);
            await _repository.SaveChangesAsync();
            throw new Exception("Concurrency conflict - please retry");
        }
    }
}