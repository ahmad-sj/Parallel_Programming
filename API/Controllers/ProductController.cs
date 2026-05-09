using API.Contracts.Products;
using Application.Admin.Products;
using Application.Admin.Products.CreateProduct;
using Application.Admin.Products.UpdateProduct;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public ProductController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        
        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] CreateProductRequest request)
        {
            var result = await _messageBus.InvokeAsync<Product>(new CreateProductCommand
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Qty = request.Qty,
            });

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            await _messageBus.InvokeAsync(new UpdateProductCommand(request.ProductId, request.ProductQty));
            return Ok("product quantity updated successfully");
        }
    }
}
