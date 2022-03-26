using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using TaxiCentral.API.Controllers;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;
using TaxiCentral.API.Services;
using TaxiCentral.API.ViewModels;
using Xunit;

namespace TaxiCentral.UnitTests
{
    public class RidesControllerTest
    {
        private Mock<IRideRepository> _rideRepository;
        private Mock<IDriverRepository> _driverRepository;
        private Mock<IIdentityService> _identityService;
        private Mock<IRouteRepository> _routeRepository;
        private Mock<IMapper> _mapper;

        public RidesControllerTest()
        {
            _rideRepository = new Mock<IRideRepository>();
            _driverRepository = new Mock<IDriverRepository>();
            _identityService = new Mock<IIdentityService>();
            _routeRepository = new Mock<IRouteRepository>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetRides_ReturnsOk()
        {
            // Arrange
            var sut = new RidesController(_rideRepository.Object, _driverRepository.Object, _identityService.Object, _mapper.Object, _routeRepository.Object);

            // Act
            var result = await sut.GetRides();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<RideViewModel>>>(result);
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetRides_ProperlyMapsObjects()
        {
            // Arrange
            _rideRepository.Setup(x => x.AllIncluding(It.IsAny<Expression<Func<Ride, object>>[]>()))
                .ReturnsAsync(new List<Ride>()
                {
                    new Ride
                    {
                        Id = Guid.NewGuid(),
                        //Driver = new Driver("Darko", "Meshkovski", "1234")
                        //{
                        //    Id = Guid.NewGuid()
                        //}
                    }
                });

            _mapper.Setup(x => x.Map<IEnumerable<RideViewModel>>(It.IsAny<List<Ride>>()))
                .Returns(new List<RideViewModel>
                {
                    new RideViewModel()
                });

            var sut = new RidesController(_rideRepository.Object, _driverRepository.Object, _identityService.Object, _mapper.Object, _routeRepository.Object);

            // Act
            var result = await sut.GetRides();

            // Assert
            var valueResult = Assert.IsAssignableFrom<IEnumerable<RideViewModel>>(result.Value);
            valueResult.Count().Should().Be(1);

            _mapper.Verify(x => x.Map<IEnumerable<RideViewModel>>(It.IsAny<List<Ride>>()), Times.Once());
        }
    }
}
