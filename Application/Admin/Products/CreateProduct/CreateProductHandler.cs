using TanvirArjel.EFCore.GenericRepository;
using Domain.Entities;

namespace Application.Admin.Products.CreateProduct
{
    public class CreateProductHandler
    {
        private readonly IRepository _repository;

        public CreateProductHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Product> Handle(CreateProductCommand command)
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
    }
}
