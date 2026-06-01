using API.Contracts.Auth;
using Application.Admin.Products.AddProduct;
using Application.Users.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMessageBus _messageBus;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);

        public AuthController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] UserRegisterationRequest request)
        {
            var result = await _messageBus.InvokeAsync<User>(new UserRegisterationCommand
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password
            });

            return Ok(result.Id);
        }
    }
}
