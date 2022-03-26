using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiCentral.API.Infrastructure.Exceptions;
using TaxiCentral.API.Infrastructure.Repositories;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDriverRepository _driverRepository;

        public AuthenticationController(IConfiguration configuration, IDriverRepository driverRepository)
        {
            _configuration = configuration;
            _driverRepository = driverRepository;
        }

        /// <summary>
        /// [Anonymous] Authenticates driver by pin
        /// </summary>
        /// <param name="pin">Driver's pin</param>
        /// <returns>JWT</returns>
        /// <remarks>tbd..</remarks>
        /// <exception cref="AppException">Application exception, usually missing configuration</exception>
        [HttpPost("driver")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> AuthenticateDriver(string? pin)
        {
            // validate user
            var driver = await _driverRepository.GetDriverByPin(pin);
            if (driver == null)
            {
                return NotFound(DriverExceptionMessage.NOT_FOUND);
            }

            // create a token
            if (string.IsNullOrWhiteSpace(_configuration["Authentication:Secret"]))
            {
                throw new AppException(AppExceptionMessage.MISSING_AUTHENTICATION_SECRET);
            }

            var securityKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:Secret"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claims
            var claimsForToken = new List<Claim>
            {
                new("sub", driver.Id.ToString()),
                new("given_name", driver.Name),
                new("family_name", driver.Surname),
                new(ClaimTypes.Role, UserType.Driver)
            };

            if (string.IsNullOrWhiteSpace(_configuration["Authentication:Issuer"]))
            {
                throw new AppException(AppExceptionMessage.MISSING_AUTHENTICATION_ISSUER);
            }

            if (string.IsNullOrWhiteSpace(_configuration["Authentication:Audience"]))
            {
                throw new AppException(AppExceptionMessage.MISSING_AUTHENTICATION_AUDIENCE);
            }

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(10),
                signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(token);
        }
    }
}
