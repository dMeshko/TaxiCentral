using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;
using TaxiCentral.API.Services;
using TaxiCentral.API.ViewModels;

namespace TaxiCentral.API.Controllers
{
    [ApiController]
    [Route("api/rides")]
    public class RidesController : ControllerBase
    {
        private readonly IRideRepository _rideRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public RidesController(IRideRepository rideRepository, IDriverRepository driverRepository, IIdentityService identityService, IMapper mapper)
        {
            _rideRepository = rideRepository;
            _driverRepository = driverRepository;
            _identityService = identityService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RideViewModel>>> GetRides()
        {
            var rides = await _rideRepository.AllIncluding(x => x.Driver);
            return Ok(_mapper.Map<IEnumerable<RideViewModel>>(rides));
        }

        [HttpGet("/api/drivers/{driverId:guid}/rides")]
        public async Task<ActionResult<IEnumerable<RideViewModel>>> GetRidesForDriver(Guid driverId)
        {
            var rides = await _rideRepository.GetAllForDriver(driverId);
            return Ok(_mapper.Map<IEnumerable<RideViewModel>>(rides));
        }

        [HttpGet("{id:guid}", Name = nameof(GetRide))]
        public async Task<ActionResult<RideViewModel>> GetRide(Guid id)
        {
            var ride = await _rideRepository.GetSingle(id);
            if (ride == null)
            {
                return NotFound();
            }

            return Ok(ride);
        }

        [HttpPost]
        public async Task<ActionResult<RideViewModel>> StartRide(StartRideViewModel model)
        {
            var driverId = _identityService.GetUserId();
            var driver = await _driverRepository.GetSingle(driverId);
            if (driver == null)
            {
                return NotFound();
            }

            var ride = _mapper.Map<Ride>(model);
            ride.Driver = driver;
            await _rideRepository.Add(ride);
            await _rideRepository.Commit();

            var rideViewModel = _mapper.Map<RideViewModel>(ride);

            return CreatedAtRoute(nameof(GetRide), new { id = ride.Id }, rideViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> FinishRide(Guid id, FinishRideViewModel model)
        {
            var ride = await _rideRepository.GetSingle(id);
            if (ride == null)
            {
                return NotFound();
            }

            if (ride.Status == RideStatus.Complete)
            {
                return BadRequest();
            }

            _mapper.Map(model, ride);

            ride.DestinationTime = DateTime.UtcNow;
            ride.Status = RideStatus.Complete;

            // auto calculate cost if not specified explicitly
            if (ride.Cost is null or 0)
            {
                var elapsedTime = ride.DestinationTime - ride.StartTime;

                const int pricePerMinute = 5; //todo: get this from app settings
                var chargePerMinute = elapsedTime.Value.Minutes * pricePerMinute;

                const int pricePerMileage = 20; //todo: get this from app settings
                var chargePerMileage = ride.Mileage * pricePerMileage;

                ride.Cost = chargePerMinute + chargePerMileage;
            }

            await _rideRepository.Update(ride);
            await _rideRepository.Commit();

            return NoContent();
        }
    }
}
