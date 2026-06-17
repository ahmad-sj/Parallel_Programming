using Application.Enums;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Products.GetProducts;
public class GetProductsHandler {
    private readonly IRepository _repository;
    private readonly IDistributedCache _cache;
    public GetProductsHandler(IRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }
    public async Task<List<Product>> Handle(GetProductsCommand command) {
        List<Product> products;

        // Try to get data from cache
        var cachedData = await _cache.GetStringAsync(CacheKeys.GetProducts);
        if (!string.IsNullOrEmpty(cachedData))
        {
            Helpers.PrintTimestamp("Cache HIT - Returning data from Redis");
            var deserializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            products = JsonSerializer.Deserialize<List<Product>>(cachedData, deserializerOptions) ?? new List<Product>();
            return products;
        }

        // Get data from database
        Helpers.PrintTimestamp("Cache MISS - Fetching from SQL Server database");
        products = await _repository.GetListAsync<Product>();

        // Serialize and save to Redis with a 5-minute expiration time
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        var serializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = global::System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var serializedData = JsonSerializer.Serialize(products, serializerOptions);
        await _cache.SetStringAsync(CacheKeys.GetProducts, serializedData, cacheOptions);
        return products;
    }
}

