using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Admin.Products.GetProducts;

public class GetProductsHandler
{
    private readonly IRepository _repository;

    public GetProductsHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Product>> Handle(GetProductsCommand command)
    {
        var products = await _repository.GetListAsync<Product>();
        return products;
    }
}
