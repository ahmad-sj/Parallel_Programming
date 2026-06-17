using Application.Enums;
using Application.Usecases.Admin.Products.UpdateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Products.IncreaseProductStock;

public class IncreaseProductStockHandler
{
    private readonly IRepository _repository;
    private readonly IDistributedCache _cache;

    public IncreaseProductStockHandler(IRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IncreaseProductStockResult> Handle(IncreaseProductStockCommand command)
    {
        try
        {
            var product = await _repository.GetAsync<Product>(p => p.Id == command.ProductId);

            Helpers.PrintTimestamp($"Current Stock: {product.Qty}");

            Helpers.PrintTimestamp("Increasing Product Stock ...");

            await Task.Delay(2000);

            product.Qty += command.IncreaseQty;

            await _repository.SaveChangesAsync();

            Helpers.PrintTimestamp($"Product Stock has been increased successfully. New Stock: {product.Qty}");

            return new IncreaseProductStockResult{ NewQty = product.Qty };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Helpers.PrintTimestamp("Product is being updated concurrently. try again later.");

            return new IncreaseProductStockResult{ NewQty = -1 };
        }
    }

    public async Task After(IncreaseProductStockCommand command)
    {
        await _cache.RemoveAsync(CacheKeys.GetProducts);
    }
}
