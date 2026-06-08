using Application.Enums;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Products.AddProducts;

public class AddProductsHandler
{
    private readonly IRepository _repository;
    private readonly IDistributedCache _cache;

    public AddProductsHandler(IRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task Before(AddProductsCommand command)
    {
        Helpers.PrintTimestamp("====================== Before Adding Products =====================");
        Helpers.PrintTimestamp($"Processing AddProducts in SQL Server...");
    }

    public async Task<List<Product>> Handle(AddProductsCommand command)
    {
        // take the list of CreateProductCommand and create a list of Product
        var products = new List<Product>();

        foreach (var createProductCommand in command.List)
        {
            var product = new Product
            {
                Name = createProductCommand.Name,
                Description = createProductCommand.Description,
                Price = createProductCommand.Price,
                Qty = createProductCommand.Qty,
            };
            products.Add(product);
        }

        // add the list of products to the database
        await _repository.AddAsync<Product>(products);
        await _repository.SaveChangesAsync();
        Helpers.PrintTimestamp($"Products Added to Database Successfully");

        return products;
    }

    public async Task After(AddProductsCommand command)
    {
        Helpers.PrintTimestamp("====================== After Adding Products ======================");

        await _cache.RemoveAsync(CacheKeys.GetProducts);
        Helpers.PrintTimestamp($"Key '{CacheKeys.GetProducts}' Has been cleared from Redis.");
    }
}
