using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaxiCentral.API.Controllers
{
    [Route("api/drivers")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetRides()
        {
            return Ok(2);
        }
    }
}
