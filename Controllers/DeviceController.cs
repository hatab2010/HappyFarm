using HappyFarm.Services;
using Microsoft.AspNetCore.Mvc;

namespace HappyFarm.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {

        [HttpGet()]
        public IEnumerable<IDevice> GetDevices([FromServices]DeviceServices deviceServices)
        {
            return deviceServices.Devices;
        }
    }
}