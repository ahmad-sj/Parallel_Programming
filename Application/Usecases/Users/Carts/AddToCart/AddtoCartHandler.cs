using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Users.Carts.AddToCart;

public class AddtoCartHandler
{
    private readonly IRepository _repository;

    public AddtoCartHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<AddtoCartResult> Handle(AddtoCartCommand command)
    {
        // check product availability
        var product = await _repository.GetQueryable<Product>().Where(x => x.Id == command.ProductId)
            .FirstOrDefaultAsync();

        if (product == null)
            throw new Exception("Product not found exception");

        if (command.Qty > product.Qty)
            throw new Exception("Product stock is insufficient");

        // check if customer already has a cart
        var cart = await _repository.GetAsync<Cart>(x => x.UserId == command.UserId, opt => opt.Include(i => i.Items).ThenInclude(i => i.Product));

        bool cartExists = cart != null;

        if (!cartExists)
        {
            cart = Cart.Create(command.UserId);
            await _repository.AddAsync<Cart>(cart);
        }

        // add product to cart
        cart!.AddItem(new CartItem
        {
            ProductId = command.ProductId,
            ProductName = product.Name,
            Qty = command.Qty,
        });

        if (cartExists)
            _repository.Update(cart);

        _repository.Update(cart);
        await _repository.SaveChangesAsync();

        Helpers.PrintTimestamp($"User '{command.UserId}' added product '{product.Id}' to cart successfully.");

        return new AddtoCartResult
        {
            CartId = cart.Id,
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
}
