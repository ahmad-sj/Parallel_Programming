using Application.Enums;
using Domain.Entities;
using JasperFx.Core;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Admin.Products.AddProduct;

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
        Console.WriteLine($"[{Environment.MachineName}] Processing AddProduct in SQL Server...");
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
        await _cache.RemoveAsync(CacheKeys.GetProducts);
        Console.WriteLine($"[{Environment.MachineName}] Key '{CacheKeys.GetProducts}' has been cleared from Redis.");
    }
}
