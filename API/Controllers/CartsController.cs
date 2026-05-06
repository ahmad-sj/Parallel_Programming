using API.Contracts.Cart;
using Application.Users.Carts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public CartsController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                var result = await _messageBus.InvokeAsync<AddtoCartResult>(new AddtoCartCommand
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    ProductName = request.ProductName,
                    Qty = request.Qty,
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
             }
        }
    }
}
