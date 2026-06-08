using Application.Enums;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Products.AddProduct;

public class AddProductHandler
{
    private readonly IRepository _repository;
    private readonly IDistributedCache _cache;

    public AddProductHandler(IRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task Before(AddProductCommand command)
    {
        Helpers.PrintTimestamp("===================== Before Adding a Product =====================");
        Helpers.PrintTimestamp("Processing AddProduct in SQL Server...");
    }

    public async Task<Product> Handle(AddProductCommand command)
    {
        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Qty = command.Qty,
        };

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        return product;
    }

    public async Task After(AddProductCommand command)
    {
        Helpers.PrintTimestamp("===================== After Adding a Product ======================");

        await _cache.RemoveAsync(CacheKeys.GetProducts);
        Helpers.PrintTimestamp($"Key '{CacheKeys.GetProducts}' has been cleared from Redis.");
    }
}
