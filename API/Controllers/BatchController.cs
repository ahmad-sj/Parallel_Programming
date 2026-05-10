using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly OrderSalesProcesser _processor;

        public BatchController(OrderSalesProcesser processor)
        {
            _processor = processor;
        }

        [HttpPost("Run")]
        public async Task<IActionResult> RunBatch()
        {
            await _processor.ProcessSalesAsync(CancellationToken.None);

            return Ok("Batch Processing Completed");
        }
    }
}
