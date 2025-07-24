using Microsoft.AspNetCore.Mvc;

namespace Authentication_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DummyController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Dummy endpoint is working!");
        }
    }
}