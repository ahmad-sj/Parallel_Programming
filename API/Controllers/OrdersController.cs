using Application.Usecases.Admin.Orders;
using Application.Usecases.Admin.Orders.GetOrderById;
using Application.Usecases.Admin.Orders.GetOrders;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public OrdersController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderById([FromBody] int orderId)
        {
            var order = await _messageBus.InvokeAsync<OrderResult>(new GetOrderByIdCommand { Id = orderId });
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _messageBus.InvokeAsync<List<OrderResult>>(new GetOrdersCommand());
            return Ok(orders);
        }
    }
}
