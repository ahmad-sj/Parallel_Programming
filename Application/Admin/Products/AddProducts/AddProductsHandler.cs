using Application.Admin.Products.AddProduct;
using Application.Enums;
using Domain.Entities;
using JasperFx.Core;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Admin.Products.AddProducts;

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
        Console.WriteLine($"[{Environment.MachineName}] Processing AddProducts in SQL Server...");
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

        return products;
    }

    public async Task After(AddProductsCommand command)
    {
        await _cache.RemoveAsync(CacheKeys.GetProducts);
        Console.WriteLine($"[{Environment.MachineName}] Key '{CacheKeys.GetProducts}' has been cleared from Redis.");
    }
}
