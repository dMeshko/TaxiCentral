using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxiCentral.API.Infrastructure.Exceptions;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;
using TaxiCentral.API.Services;
using TaxiCentral.API.ViewModels;
using Route = TaxiCentral.API.Models.Route;

namespace TaxiCentral.API.Controllers
{
    [Route("api/routes")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IDriverRepository _driverRepository;

        public RoutesController(IRouteRepository routeRepository, IMapper mapper, IIdentityService identityService, IDriverRepository driverRepository)
        {
            _routeRepository = routeRepository;
            _mapper = mapper;
            _identityService = identityService;
            _driverRepository = driverRepository;
        }

        /// <summary>
        /// [Dispatcher] Gets all unassigned (pending) routes in the system
        /// </summary>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Dispatcher)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RouteViewModel>>> GetAllPendingRoutes()
        {
            var routes = await _routeRepository.GetAllPendingRoutes();
            if (!routes.Any())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<RouteViewModel>>(routes));
        }

        /// <summary>
        /// [Client, Driver, Dispatcher] Gets specific driver by id
        /// </summary>
        /// <param name="id" cref="Guid">Route's id</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = $"{UserType.Client}, {UserType.Driver}, {UserType.Dispatcher}")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RouteViewModel>> GetRoute(Guid id)
        {
            var route = await _routeRepository.GetSingle(x => x.Id == id, x => x.Ride);
            if (route == null)
            {
                return NotFound(RouteExceptionMessage.NOT_FOUND);
            }

            return Ok(_mapper.Map<RouteViewModel>(route));
        }

        /// <summary>
        /// [Client, Dispatcher] Adds a route into the system
        /// </summary>
        /// <param name="model" cref="AddRouteViewModel">New route data</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = $"{UserType.Client}, {UserType.Dispatcher}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RouteViewModel>> AddRoute(AddRouteViewModel model)
        {
            var route = _mapper.Map<Route>(model);
            await _routeRepository.Add(route);
            await _routeRepository.Commit();

            var routeViewModel = _mapper.Map<RouteViewModel>(route);
            return CreatedAtRoute(nameof(GetRoute), new { id = route.Id }, routeViewModel);
        }

        /// <summary>
        /// [Driver] Adds and accepts a route, also starts a ride (used by the driver, when picking up ad-hoc passenger)
        /// </summary>
        /// <param name="model" cref="AddRouteViewModel">New route data</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Driver)]
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RideViewModel>> AddAndStartRoute(AddRouteViewModel model)
        {
            var driverId = _identityService.GetUserId();
            var driver = await _driverRepository.GetSingle(driverId);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            // add route
            var route = _mapper.Map<Route>(model);
            
            // accept route
            route.Status = RouteStatus.Current;
            route.Driver = driver;

            // start ride
            var ride = new Ride
            {
                ActualStartingPoint = _mapper.Map<LatLng>(model.TargetStartingPoint)
            };
            route.Ride = ride;

            await _routeRepository.Add(route);
            await _routeRepository.Commit();

            var rideViewModel = _mapper.Map<RideViewModel>(ride);
            return CreatedAtRoute(nameof(RidesController.GetRide), new { id = ride.Id }, rideViewModel);
        }

        /// <summary>
        /// [Driver] The driver accepts route
        /// </summary>
        /// <param name="model" cref="AcceptRideViewModel">Accept ride model</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Driver)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AcceptRide(AcceptRideViewModel model)
        {
            var driverId = _identityService.GetUserId();
            var driver = await _driverRepository.GetSingle(driverId);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            var route = await _routeRepository.GetSingle(model.RouteId);
            if (route == null)
            {
                return NotFound(RouteExceptionMessage.NOT_FOUND);
            }

            route.Status = RouteStatus.Current;
            route.EstimatedTimeOfArrival = model.EstimatedTimeOfArrival;
            route.Driver = driver;

            await _routeRepository.Update(route);
            await _routeRepository.Commit();

            return NoContent();
        }

        /// <summary>
        /// [Driver] The driver has arrived at the requested location
        /// </summary>
        /// <param name="id" cref="Guid">Route's id</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Driver)]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DriverArrivedAtLocation(Guid id)
        {
            var driverId = _identityService.GetUserId();
            var driver = await _driverRepository.GetSingle(driverId);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            var route = await _routeRepository.GetSingle(id);
            if (route == null)
            {
                return NotFound(RouteExceptionMessage.NOT_FOUND);
            }

            route.Status = RouteStatus.WaitingForClient;
            route.ArrivalTime = DateTime.UtcNow;

            await _routeRepository.Update(route);
            await _routeRepository.Commit();

            return NoContent();
        }

        /// <summary>
        /// [Client, Driver, Dispatcher] The route is canceled for some reason
        /// </summary>
        /// <param name="model">Route termination model</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = $"{UserType.Client}, {UserType.Driver}, {UserType.Dispatcher}")]
        [HttpPut("terminate")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> TerminateRoute(TerminateRouteViewModel model)
        {
            var route = await _routeRepository.GetSingle(model.RouteId);
            if (route == null)
            {
                return NotFound(RouteExceptionMessage.NOT_FOUND);
            }

            route.Status = RouteStatus.Voided;
            route.VoidReason = model.Reason;

            await _routeRepository.Update(route);
            await _routeRepository.Commit();

            return NoContent();
        }
    }
}
