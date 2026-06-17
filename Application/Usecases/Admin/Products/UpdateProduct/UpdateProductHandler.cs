using Application.Enums;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Products.UpdateProduct;
public class UpdateProductHandler {
    private readonly IRepository _repository;
    private readonly IDistributedCache _cache;

    public UpdateProductHandler(IRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task Handle(UpdateProductCommand command) {
        Helpers.PrintTimestamp($"Updating product '{command.ProductId}' in SQL Server ...");
        await Task.Delay(4000);
        var product = await _repository.GetAsync<Product>(p => p.Id == command.ProductId);
        product.Qty = command.ProductQty;
        await _repository.SaveChangesAsync();
    }

    public async Task After(UpdateProductCommand command) {
        Helpers.PrintTimestamp("===================== After Updating a Product ====================");
        await _cache.RemoveAsync(CacheKeys.GetProducts);
        Helpers.PrintTimestamp($"Key '{CacheKeys.GetProducts}' Has been cleared from Redis.");
    }
}


