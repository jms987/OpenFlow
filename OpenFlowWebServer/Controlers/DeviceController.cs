using Microsoft.AspNetCore.Mvc;

namespace OpenFlowWebServer.Controlers
{
    public class DeviceController : Controller
    {
        [HttpPost("/devices/register")]
        public IActionResult RegisterDevice()
        {
            // Logic to register a device
            return Json(new { message = "Device registered successfully" });
        }
        
        [HttpGet("/devices/params")]
        public IActionResult GetParams()
        {
            // Logic to delete a device
            return Json(new { message = "Device deleted successfully" });
        }
    }
}
