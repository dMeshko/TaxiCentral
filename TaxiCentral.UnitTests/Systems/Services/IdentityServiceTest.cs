using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using TaxiCentral.API.Infrastructure.Exceptions;
using TaxiCentral.API.Services;
using TaxiCentral.UnitTests.Fixtures;
using Xunit;

namespace TaxiCentral.UnitTests.Systems.Services
{
    public class IdentityServiceTest
    {
        [Fact]
        public void GetUserId_Returns_ThrowsException_ForTokenWithoutSubClaim()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext!.User.FindFirst("sub"))
                .Returns((Claim?)null);

            var identityService = new IdentityService(httpContextAccessor.Object);

            // Act
            var result = () => identityService.GetUserId();

            // Assert
            result.Should().Throw<AppException>()
                .WithMessage(AppExceptionMessage.INVALID_TOKEN_MISSING_SUB);
        }

        [Fact]
        public void GetUserId_Returns_Id()
        {
            // Arrange
            var fakeDriver = DriversFixture.GetFakeDriver();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext!.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim("sub", fakeDriver.Id.ToString()));

            var identityService = new IdentityService(httpContextAccessor.Object);

            // Act
            var result = identityService.GetUserId();

            // Assert
            result.Should().Be(fakeDriver.Id.ToString());
        }
    }
}
