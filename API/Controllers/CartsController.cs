using API.Contracts.Cart;
using Application.Usecases.Users.Carts.AddToCart;
using Application.Usecases.Users.Carts.Checkout;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
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
                    Qty = request.Qty,
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var result = await _messageBus.InvokeAsync<CheckoutResult>(new CheckoutCommand
            {
                UserId = request.UserId,
                PaymentSuccess = request.PaymentSuccess,
                PaymentTime = request.PaymentTime,
            });

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
