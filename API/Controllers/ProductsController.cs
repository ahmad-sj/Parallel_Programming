using API.Contracts.Products;
using Application.Usecases.Admin.Products.AddProduct;
using Application.Usecases.Admin.Products.AddProducts;
using Application.Usecases.Admin.Products.DeleteProduct;
using Application.Usecases.Admin.Products.GetProducts;
using Application.Usecases.Admin.Products.UpdateProduct;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public ProductsController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }


        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] AddProductRequest request)
        {
            var result = await _messageBus.InvokeAsync<Product>(new AddProductCommand
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Qty = request.Qty,
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<Product>>> AddProducts([FromBody] AddProductsRequest request)
        {
            var addProductsCommand  = new AddProductsCommand
            {
                List = request.List.Select(p => new AddProductCommand
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Qty = p.Qty,
                }).ToList()
            };
            var result = await _messageBus.InvokeAsync<List<Product>>(addProductsCommand);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var result = await _messageBus.InvokeAsync<List<Product>>(new GetProductsCommand());
           return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            await _messageBus.InvokeAsync(new UpdateProductCommand(request.ProductId, request.ProductQty));
            return Ok("product quantity updated successfully");
        }

        [HttpDelete]
        public async Task DeleteProduct([FromBody] DeleteProductRequest request)
        {
            await _messageBus.InvokeAsync(new DeleteProductCommand(request.Id));
        }
    }
}
