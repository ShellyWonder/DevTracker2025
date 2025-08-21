using Microsoft.AspNetCore.Mvc;

namespace WonderDevTracker.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("hello")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetGreeting()
        {
            return Ok("Hello from DevTracker API!");
        }
    }
}
