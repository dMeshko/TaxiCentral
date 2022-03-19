using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaxiCentral.API.Infrastructure.Exceptions;
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

        /// <summary>
        /// Get all rides in the system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RideViewModel>>> GetRides()
        {
            var rides = await _rideRepository.AllIncluding(x => x.Driver);
            if (!rides.Any())
            {
                return NotFound(RideExceptionMessage.NO_RIDES);
            }
            
            return Ok(_mapper.Map<IEnumerable<RideViewModel>>(rides));
        }

        [HttpGet("/api/drivers/{driverId:guid}/rides")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RideViewModel>>> GetRidesForDriver(Guid driverId)
        {
            var rides = await _rideRepository.GetAllForDriver(driverId);
            if (!rides.Any())
            {
                return NotFound(RideExceptionMessage.NO_RIDES);
            }

            return Ok(_mapper.Map<IEnumerable<RideViewModel>>(rides));
        }

        [HttpGet("{id:guid}", Name = nameof(GetRide))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RideViewModel>> GetRide(Guid id)
        {
            var ride = await _rideRepository.GetSingle(x => x.Id == id, x => x.Driver);
            if (ride == null)
            {
                return NotFound(RideExceptionMessage.NOT_FOUND);
            }
             
            return Ok(_mapper.Map<RideViewModel>(ride));
        }

        ////todo
        //public async Task<ActionResult<RideViewModel>> BroadcastRide(BroadcastRideViewModel model)
        //{
        //    var areaRadius = 3000; // 3000m/3km //todo: get this from settings
        //    var driverInArea = _driverRepository.GetDriversInArea(_mapper.Map<LatLng>(model.TargetStartingPoint), areaRadius);
        //    return null;
        //}

        //todo: add reon
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RideViewModel>> StartRide(StartRideViewModel model)
        {
            var driverId = _identityService.GetUserId();
            var driver = await _driverRepository.GetSingle(driverId);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            var ride = _mapper.Map<Ride>(model);
            ride.Driver = driver;
            ride.Status = RideStatus.Current;
            await _rideRepository.Add(ride);
            await _rideRepository.Commit();

            var rideViewModel = _mapper.Map<RideViewModel>(ride);

            return CreatedAtRoute(nameof(GetRide), new { id = ride.Id }, rideViewModel);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> FinishRide(Guid id, FinishRideViewModel model)
        {
            var ride = await _rideRepository.GetSingle(id);
            if (ride == null)
            {
                return NotFound(RideExceptionMessage.NOT_FOUND);
            }

            if (ride.Status == RideStatus.Complete)
            {
                return BadRequest(RideExceptionMessage.ALREADY_COMPLETE);
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
