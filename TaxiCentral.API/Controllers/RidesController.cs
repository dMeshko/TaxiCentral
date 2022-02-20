using Microsoft.AspNetCore.Mvc;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Controllers
{
    [ApiController]
    [Route("api/rides")]
    public class RidesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<RideDto>> GetRides()
        {
            return Ok(RidesDataStore.Current.Rides);
        }

        [HttpGet("{id:guid}", Name = nameof(GetRide))]
        public ActionResult<RideDto> GetRide(Guid id)
        {
            var idd = User.Claims.FirstOrDefault(x => x.Type == "sub")!.Value;
            var ride = RidesDataStore.Current.Rides.FirstOrDefault(x => x.Id == id);
            if (ride == null)
            {
                return NotFound();
            }

            return Ok(ride);
        }

        [HttpPost("api/driver/{driverId:guid}/rides")]
        public ActionResult<RideDto> ScheduleRide(Guid driverId, ScheduleRideViewModel model)
        {
            // demo: remove later, makes no sense
            return CreatedAtRoute(nameof(GetRide), Guid.NewGuid(), model);
        }
    }
}
