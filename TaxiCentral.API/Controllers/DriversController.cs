using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TaxiCentral.API.Infrastructure.Exceptions;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;
using TaxiCentral.API.Services;
using TaxiCentral.API.ViewModels;

namespace TaxiCentral.API.Controllers
{
    [Route("api/drivers")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class DriversController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        
        public DriversController(IDriverRepository driverRepository, IMapper mapper, IIdentityService identityService)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
            _identityService = identityService;
        }

        /// <summary>
        /// [Dispatcher, Admin] Gets all drivers in the system
        /// </summary>
        /// <returns cref="IEnumerable{DriverViewModel}">tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = $"{UserType.Dispatcher}, {UserType.Admin}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<DriverViewModel>> GetDrivers()
        {
            var drivers = _driverRepository.GetAll().ToList();
            if (!drivers.Any())
            {
                return NotFound(DriverExceptionMessage.NO_DRIVERS);
            }

            return Ok(_mapper.Map<IEnumerable<DriverViewModel>>(drivers));
        }

        /// <summary>
        /// [Dispatcher, Admin] Gets specific driver by id
        /// </summary>
        /// <param name="id" cref="Guid">Driver's id</param>
        /// <returns cref="DriverViewModel">tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = $"{UserType.Dispatcher}, {UserType.Admin}")]
        [HttpGet("{id:guid}", Name = nameof(GetDriver))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DriverViewModel>> GetDriver(Guid id)
        {
            var driver = await _driverRepository.GetSingle(id);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            return Ok(_mapper.Map<DriverViewModel>(driver));
        }

        /// <summary>
        /// [Admin] Adds new driver into the system
        /// </summary>
        /// <param name="model" cref="CreateDriverViewModel">New driver data</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Admin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<DriverViewModel>> AddDriver(CreateDriverViewModel model)
        {
            var driver = _mapper.Map<Driver>(model);

            if (await _driverRepository.AlreadyExists(driver))
            {
                return BadRequest(DriverExceptionMessage.ALREADY_EXISTS);
            }

            await _driverRepository.Add(driver);
            await _driverRepository.Commit();

            var driverViewModel = _mapper.Map<DriverViewModel>(driver);

            return CreatedAtRoute(nameof(GetDriver), new { id = driver.Id }, driverViewModel);
        }

        /// <summary>
        /// [Admin] Updates driver
        /// </summary>
        /// <param name="id" cref="Guid">Driver's id</param>
        /// <param name="patchDocument" cref="JsonPatchDocument{UpdateDriverViewModel}">Data that needs updating</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Admin)]
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateDriver(Guid id, JsonPatchDocument<UpdateDriverViewModel> patchDocument)
        {
            var driver = await _driverRepository.GetSingle(id);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            var driverViewModel = _mapper.Map<UpdateDriverViewModel>(driver);
            patchDocument.ApplyTo(driverViewModel);

            if (!TryValidateModel(driverViewModel))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(driverViewModel, driver);

            if (await _driverRepository.AlreadyExists(driver))
            {
                return BadRequest(DriverExceptionMessage.ALREADY_EXISTS);
            }

            await _driverRepository.Update(driver);
            await _driverRepository.Commit();

            return NoContent();
        }

        /// <summary>
        /// [Admin] Deletes a driver
        /// </summary>
        /// <param name="id">Driver's id</param>
        /// <returns>tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Admin)]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDriver(Guid id)
        {
            var driver = await _driverRepository.GetSingle(id);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            await _driverRepository.Delete(driver);
            await _driverRepository.Commit();

            return NoContent();
        }

        /// <summary>
        /// [Driver] Gets currently logged in driver info
        /// </summary>
        /// <returns cref="DriverViewModel">tbd..</returns>
        /// <remarks>tbd..</remarks>
        [Authorize(Roles = UserType.Driver)]
        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<ActionResult<DriverViewModel>> GetCurrentDriver()
        {
            var currentDriverId = _identityService.GetUserId();
            return GetDriver(currentDriverId);
        }
    }
}
