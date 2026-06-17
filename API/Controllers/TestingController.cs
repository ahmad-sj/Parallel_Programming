using Application;
using Application.Services.DailySales;
using Application.Usecases.Admin.Database;
using Application.Usecases.Admin.Users.GetUsersIds;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Wolverine;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly DailySalesParallelProcesser _parProcessor;
        private readonly DailySalesSequentialProcessor _seqProcessor;

        public TestingController(
            IMessageBus messageBus,
            DailySalesParallelProcesser processor,
            DailySalesSequentialProcessor seqProcessor)
        {
            _messageBus = messageBus;
            _parProcessor = processor;
            _seqProcessor = seqProcessor;
        }
                
        [HttpGet("sales-in-sequence")]
        public async Task<IActionResult> GetDailySalesInSequence()
        {
            await _seqProcessor.ProcessSalesAsync(CancellationToken.None);
            return Ok("Sequential Sales Processing Completed");
        }

        [HttpGet("sales-in-parallel")]
        public async Task<IActionResult> GetDailySalesInParallel()
        {
            await _parProcessor.ProcessSalesAsync(CancellationToken.None);
            return Ok("Parallel Sales Processing Completed");
        }

        [HttpGet("short-request")]
        public async Task<IActionResult> ShortRequest()
        {
            // Environment.MachineName returns the unique container ID inside Docker
            var containerId = Environment.MachineName;
            // sum the ascii values of the containerId characters to create a simple hash
            var hash = containerId.Sum(c => (int)c);
            Helpers.PrintTimestamp("Short request received in container with id: " + hash);
            return Ok(new { message = "Hello from container!", container_id = containerId, simpleHash = hash });
        }

        [HttpGet("long-request")]
        public async Task<IActionResult> LongRequest()
        {
            // Environment.MachineName returns the unique container ID inside Docker
            var containerId = Environment.MachineName;
            // sum the ascii values of the containerId characters to create a simple hash
            var hash = containerId.Sum(c => (int)c);
            Helpers.PrintTimestamp("Long request received in container with id: " + hash);
            await Task.Delay(20000); // Simulate a long request by delaying for 20 seconds
            return Ok(new { message = "Hello from container!", container_id = containerId, simpleHash = hash });
        }
                        
        [HttpGet("get-users-ids")]
        public async Task<IActionResult> GetUsersIds()
        {
            var result = await _messageBus.InvokeAsync<GetUsersIdsResult>(new GetUsersIdsCommand());
            return Ok(result);
        }

        [HttpPost("clear-database")]
        public async Task<IActionResult> ClearDatabase()
        {
            await _messageBus.InvokeAsync(new ClearDatabaseCommand());
            return Ok(new { message = "Database cleared successfully!" });
        }
    }
}
