using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Admin.Products.DeleteProduct;

public class DeleteProductHandler
{
    IRepository _repository;

    public DeleteProductHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteProductCommand command)
    {
        var product = await _repository.GetByIdAsync<Product>(command.Id);

        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        _repository.Remove<Product>(product);

        await _repository.SaveChangesAsync();
    }
}
