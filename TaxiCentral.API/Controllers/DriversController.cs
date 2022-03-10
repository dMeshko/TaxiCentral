using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<IEnumerable<DriverViewModel>> GetDrivers()
        {
            var drivers = _driverRepository.GetAll().ToList();
            return Ok(_mapper.Map<IEnumerable<DriverViewModel>>(drivers));
        }

        [HttpGet("{id:guid}", Name = nameof(GetDriver))]
        public async Task<ActionResult<DriverViewModel>> GetDriver(Guid id)
        {
            var driver = await _driverRepository.GetSingle(id);
            if (driver == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DriverViewModel>(driver));
        }

        [HttpPost]
        public async Task<ActionResult<DriverViewModel>> AddDriver(CreateDriverViewModel model)
        {
            var driver = _mapper.Map<Driver>(model);

            if (await _driverRepository.AlreadyExists(driver))
            {
                return BadRequest();
            }

            await _driverRepository.Add(driver);
            await _driverRepository.Commit();

            var driverViewModel = _mapper.Map<DriverViewModel>(driver);

            return CreatedAtRoute(nameof(GetDriver), new { id = driver.Id }, driverViewModel);
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult> UpdateDriver(Guid id, JsonPatchDocument<UpdateDriverViewModel> patchDocument)
        {
            var driver = await _driverRepository.GetSingle(id);
            if (driver == null)
            {
                return NotFound();
            }

            var driverViewModel = _mapper.Map<UpdateDriverViewModel>(driver);
            patchDocument.ApplyTo(driverViewModel);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(driverViewModel))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(driverViewModel, driver);

            if (await _driverRepository.AlreadyExists(driver))
            {
                return BadRequest();
            }

            await _driverRepository.Update(driver);
            await _driverRepository.Commit();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteDriver(Guid id)
        {
            var driver = await _driverRepository.GetSingle(id);
            if (driver == null)
            {
                return NotFound();
            }

            await _driverRepository.Delete(driver);
            await _driverRepository.Commit();

            return NoContent();
        }
    }
}
