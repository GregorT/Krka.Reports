using Microsoft.AspNetCore.Mvc;

namespace Krka.Reports.ApiGateway.Controllers
{
    [ApiController]
    [Route("sinh")]
    public class SinhController : ControllerBase
    {

        [HttpGet]
        public IEnumerable<string> Get()
        {

        }
    }
}