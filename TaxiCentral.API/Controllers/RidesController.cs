using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IRouteRepository _routeRepository;

        public RidesController(IRideRepository rideRepository, IDriverRepository driverRepository, IIdentityService identityService, IMapper mapper, IRouteRepository routeRepository)
        {
            _rideRepository = rideRepository;
            _driverRepository = driverRepository;
            _identityService = identityService;
            _mapper = mapper;
            _routeRepository = routeRepository;
        }

        [Authorize(Roles = UserType.Dispatcher)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RideViewModel>>> GetRides()
        {
            //var rides = await _rideRepository.AllIncluding(x => x.Driver);
            var rides = _rideRepository.GetAll();
            if (!rides.Any())
            {
                return NotFound(RideExceptionMessage.NO_RIDES);
            }
            
            return Ok(_mapper.Map<IEnumerable<RideViewModel>>(rides));
        }

        [Authorize(Roles = UserType.Dispatcher)]
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

        [HttpGet("/api/drivers/{driverId:guid}/current-rides")]
        public async Task<ActionResult<IEnumerable<RideViewModel>>> GetCurrentRidesForDriver(Guid driverId)
        {
            var rides = await _rideRepository.GetAllCurrentForDriver(driverId);
            if (!rides.Any())
            {
                return NotFound(RideExceptionMessage.NO_RIDES);
            }

            var stack = new Stack<RideViewModel>();
            for (var i = 0; i < rides.Count; i++)
            {
                var currentRide = rides[i];
                stack.Push(_mapper.Map<RideViewModel>(currentRide));
                if (i + 1 < rides.Count)
                {
                    var nextRide = rides[i + 1];
                    var provisionalRide = new Ride
                    {
                        //Driver = currentRide.Driver,
                        ActualStartingPoint = currentRide.ActualStartingPoint,
                        ActualDestinationPoint = nextRide.ActualDestinationPoint,
                        //TargetStartingPoint = currentRide.TargetStartingPoint,
                        //TargetDestinationPoint = nextRide.TargetDestinationPoint
                    };

                    // add provisional ride
                    stack.Push(_mapper.Map<RideViewModel>(provisionalRide));
                }
            }

            return Ok(stack.Reverse().ToList());
        }

        [HttpGet("{id:guid}", Name = nameof(GetRide))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RideViewModel>> GetRide(Guid id)
        {
            var ride = await _rideRepository.GetSingle(x => x.Id == id);
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
        /// <summary>
        /// [Driver] The passenger is in the cab, and the ride has started
        /// </summary>
        /// <param name="routeId" cref="Guid">Route's id</param>
        /// <param name="model" cref="StartRideViewModel">Start ride model</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Driver)]
        [HttpPost("/api/routes/{routeId:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RideViewModel>> StartRide(Guid routeId, StartRideViewModel model)
        {
            var route = await _routeRepository.GetSingle(routeId);
            if (route == null)
            {
                return NotFound(RouteExceptionMessage.NOT_FOUND);
            }

            var ride = _mapper.Map<Ride>(model);
            ride.TimeOfArrival = (route.ArrivalTime!.Value - route.ReportedAt).Minutes;
            ride.WaitingTime = (ride.StartTime - route.ArrivalTime!.Value).Minutes;
            route.Ride = ride;

            await _rideRepository.Add(ride);
            await _rideRepository.Commit();

            var rideViewModel = _mapper.Map<RideViewModel>(ride);

            return CreatedAtRoute(nameof(GetRide), new { id = ride.Id }, rideViewModel);
        }

        /// <summary>
        /// [Driver] The driver has finished the ride
        /// </summary>
        /// <param name="id" cref="Guid">Ride's id</param>
        /// <param name="model" cref="FinishRideViewModel">Finish route model</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Driver)]
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

                var chargeForWaiting = false; //todo: get this from app settings
                if (chargeForWaiting)
                {
                    var waitingChargePerMinute = ride.WaitingTime!.Value * pricePerMinute;
                    ride.Cost += waitingChargePerMinute;
                }
            }

            await _rideRepository.Update(ride);
            await _rideRepository.Commit();

            return NoContent();
        }
    }
}
