using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        [HttpGet("test-loadbalancer")]
        public IActionResult TestLoadBalancer()
        {
            // Environment.MachineName returns the unique container ID inside Docker
            var containerId = Environment.MachineName;

            // sum the ascii values of the containerId characters to create a simple hash
            var hash = containerId.Sum(c => (int)c);

            Console.WriteLine("Hello from container! " + hash);

            return Ok(new { message = "Hello from container!", container_id = containerId, simpleHash = hash });
        }
    }
}
