using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Admin.Products.UpdateProduct;

public class UpdateProductHandler
{
    private readonly IRepository _repository;

    public UpdateProductHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateProductCommand command)
    {
        Console.WriteLine("Updating product ...");

        await Task.Delay(2000);

        var product = await _repository.GetAsync<Product>(p => p.Id == command.ProductId);

        product.Qty = command.ProductQty;

        await _repository.SaveChangesAsync();

    }
}
