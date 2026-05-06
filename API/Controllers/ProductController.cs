using API.Contracts.Products;
using Application.Admin.Products;
using Application.Admin.Products.CreateProduct;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public ProductController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        // POST: api/product
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

        // GET: api/product/{id}
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Product>> GetProductById(int id)
        //{
        //    var product = _products.Find(p => p.Id == id);

        //    if (product == null)
        //    {
        //        return NotFound($"Product with ID {id} not found.");
        //    }

        //    return Ok(product);
        //}

        //// Optional: GET all products
        //[HttpGet]
        //public async Task<ActionResult<List<Product>>> GetAllProducts()
        //{
        //    //return Ok(_products);
        //    return Ok(_products);
        //}
    }
}
