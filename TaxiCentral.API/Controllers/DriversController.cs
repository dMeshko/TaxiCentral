using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TaxiCentral.API.Infrastructure.Exceptions;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;
using TaxiCentral.API.ViewModels;

namespace TaxiCentral.API.Controllers
{
    [Route("api/drivers")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;

        public DriversController(IDriverRepository driverRepository, IMapper mapper)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
        }

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

        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateDriver(Guid id, JsonPatchDocument<UpdateDriverViewModel> patchDocument)
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

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteDriver(Guid id)
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
    }
}
