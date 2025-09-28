using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OpenFlowWebServer.Repository;

namespace OpenFlowWebServer.Controllers
{
    [ApiController]
    [Route("devices")]
    [ApiVersion("1.0")]
    public class DeviceController : ControllerBase
    {
        private readonly IQueueRepository _queueRepository;

        public DeviceController(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        [HttpPost("register")]
        public IActionResult RegisterDevice()
        {
            // Logic to register a device
            return Ok(new { message = "Device registered successfully" });
        }


        [HttpGet("params")]
        public async Task<IActionResult> GetParams()
        {
            // Logic to delete a device
            var result = await _queueRepository.DequeueAsync("projectid28efaad9-4f8a-4629-829b-71dc9e40b357parameters");
            return Ok(result);
            // return Ok(new { message = "Device deleted successfully" });
        }
    }
}
