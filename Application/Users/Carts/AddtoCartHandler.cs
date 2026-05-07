using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;
using Wolverine;
using Wolverine;
using Application.Notifications;
namespace Application.Users.Carts
{
    public class AddtoCartHandler
    {
        private readonly IRepository _repository;
        private readonly IMessageBus _messageBus;

        public AddtoCartHandler(
            IRepository repository,
            IMessageBus messageBus)
        {
            _repository = repository;
            _messageBus = messageBus;
        }
        public async Task<AddtoCartResult> Handle(AddtoCartCommand command)
        {
            try
            {
                // check product availability
                var product = await _repository.GetQueryable<Product>().Where(x => x.Id == command.ProductId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    throw new Exception("Product not found exception");
                }

                if (command.Qty > product.Qty)
                {
                    throw new Exception("Product stock is not sufficient");
                }

                await Task.Delay(10_000);

                // deduct qty from stock
                product.Qty -= command.Qty;

                // check if customer already has a cart
                var cart = await _repository.GetAsync<Cart>(x => x.UserId == command.UserId);

                var isNewCart = cart == null;
                if (isNewCart)
                {
                    cart = Cart.Create(command.UserId);
                    await _repository.AddAsync<Cart>(cart);
                }

                // add product to cart
                cart!.Items.Add(new CartItem
                {
                    ProductId = command.ProductId,
                    ProductName = command.ProductName,
                    Qty = command.Qty,
                });

                if (!isNewCart)
                {
                    _repository.Update(cart);
                }

                _repository.Update(product);
                await _repository.SaveChangesAsync();
                
                
                await _messageBus.PublishAsync(
    new SendOrderNotificationCommand
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
                var currentValues = await entry.GetDatabaseValuesAsync();

                if (currentValues == null)
                    throw new Exception("Entity was deleted");

                // Merge changes - your logic here
                throw new Exception("Concurrency violation resolved");

                entry.OriginalValues.SetValues(currentValues);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
