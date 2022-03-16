using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Configuration;
using System.Threading.Tasks;
using TaxiCentral.API.Controllers;
using TaxiCentral.API.Infrastructure.Exceptions;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;
using Xunit;

namespace TaxiCentral.UnitTests
{
    public class AuthenticationControllerTest
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IDriverRepository> _driverRepository;

        public AuthenticationControllerTest()
        {
            _configuration = new Mock<IConfiguration>();
            _driverRepository = new Mock<IDriverRepository>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("1111")]
        public async Task AuthenticateDriver_Returns_NotFound_ForWrongPin(string pin)
        {
            // Arrange
            var fakeDriver = GetFakeDriver();

            _driverRepository.Setup(x => x.GetDriverByPin(fakeDriver.Pin))
                .ReturnsAsync(fakeDriver);

            var sut = new AuthenticationController(_configuration.Object, _driverRepository.Object);

            // Act
            var result = await sut.AuthenticateDriver(pin);

            // Assert
            result.Should().BeOfType<ActionResult<string>>()
                .Subject.Result.Should().BeOfType<NotFoundObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(DriverExceptionMessage.NOT_FOUND);
        }

        [Fact]
        public async Task AuthenticateDriver_Returns_Ok()
        {
            // Arrange
            var fakeDriver = GetFakeDriver();

            _driverRepository.Setup(x => x.GetDriverByPin(fakeDriver.Pin))
                .ReturnsAsync(fakeDriver);

            _configuration.Setup(x => x["Authentication:Secret"])
                .Returns("generateRandomGuildAndBase64EncodeIt");
            _configuration.Setup(x => x["Authentication:Issuer"])
                .Returns("someIssuer");
            _configuration.Setup(x => x["Authentication:Audience"])
                .Returns("someAudience");

            var sut = new AuthenticationController(_configuration.Object, _driverRepository.Object);

            // Act
            var result = await sut.AuthenticateDriver(fakeDriver.Pin);

            // Assert
            result.Should().BeOfType<ActionResult<string>>()
                .Subject.Result.Should().BeOfType<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task AuthenticateDriver_ThrowsException_ForMissingAuthenticationSecretConfiguration()
        {
            // Arrange
            var fakeDriver = GetFakeDriver();

            _driverRepository.Setup(x => x.GetDriverByPin(fakeDriver.Pin))
                .ReturnsAsync(fakeDriver);

            _configuration.Setup(x => x["Authentication:Issuer"])
                .Returns("someIssuer");
            _configuration.Setup(x => x["Authentication:Audience"])
                .Returns("someAudience");

            var sut = new AuthenticationController(_configuration.Object, _driverRepository.Object);

            // Act
            var taskResult = () => sut.AuthenticateDriver(fakeDriver.Pin);

            // Assert
            await taskResult.Should().ThrowAsync<AppException>()
                .WithMessage(AppExceptionMessage.MISSING_AUTHENTICATION_SECRET);
        }

        [Fact]
        public async Task AuthenticateDriver_ThrowsException_ForMissingAuthenticationIssuerConfiguration()
        {
            // Arrange
            var fakeDriver = GetFakeDriver();

            _driverRepository.Setup(x => x.GetDriverByPin(fakeDriver.Pin))
                .ReturnsAsync(fakeDriver);

            _configuration.Setup(x => x["Authentication:Secret"])
                .Returns("generateRandomGuildAndBase64EncodeIt");
            _configuration.Setup(x => x["Authentication:Audience"])
                .Returns("someAudience");

            var sut = new AuthenticationController(_configuration.Object, _driverRepository.Object);

            // Act
            var taskResult = () => sut.AuthenticateDriver(fakeDriver.Pin);

            // Assert
            await taskResult.Should().ThrowAsync<AppException>()
                .WithMessage(AppExceptionMessage.MISSING_AUTHENTICATION_ISSUER);
        }

        [Fact]
        public async Task AuthenticateDriver_ThrowsException_ForMissingAuthenticationAudienceConfiguration()
        {
            // Arrange
            var fakeDriver = GetFakeDriver();

            _driverRepository.Setup(x => x.GetDriverByPin(fakeDriver.Pin))
                .ReturnsAsync(fakeDriver);

            _configuration.Setup(x => x["Authentication:Secret"])
                .Returns("generateRandomGuildAndBase64EncodeIt");
            _configuration.Setup(x => x["Authentication:Issuer"])
                .Returns("someIssuer");

            var sut = new AuthenticationController(_configuration.Object, _driverRepository.Object);

            // Act
            var taskResult = () => sut.AuthenticateDriver(fakeDriver.Pin);

            // Assert
            await taskResult.Should().ThrowAsync<AppException>()
                .WithMessage(AppExceptionMessage.MISSING_AUTHENTICATION_AUDIENCE);
        }

        public Driver GetFakeDriver() =>
            new("Darko", "Meshkovski", "1234")
            {
                Id = Guid.NewGuid()
            };
    }
}
