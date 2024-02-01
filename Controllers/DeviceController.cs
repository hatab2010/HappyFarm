using HappyFarm.Models;
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

    [ApiController]
    [Route("[controller]")]
    public class BotController : ControllerBase
    {
        [HttpPost()]
        public async Task<ActionResult> AddToWatch([FromBody]IPost post, [FromServices]BotServices bot)
        {
            bot.AddToWatch(post);
            Ok();
        }
    }
}