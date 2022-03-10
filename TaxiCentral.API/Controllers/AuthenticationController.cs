using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiCentral.API.Infrastructure.Repositories;

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
            _configuration = configuration 
                             ?? throw new ArgumentNullException(nameof(configuration));
            _driverRepository = driverRepository;
        }

        [HttpPost("driver")]
        public async Task<ActionResult<string>> AuthenticateDriver(string pin)
        {
            // validate user
            var user = await _driverRepository.GetSingle(x => x.Pin == pin);
            if (user == null)
            {
                return Unauthorized();
            }

            // create a token
            var securityKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:Secret"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claims
            var claimsForToken = new List<Claim>
            {
                new("sub", user.Id.ToString()),
                new("given_name", user.Name),
                new("family_name", user.Surname)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(token);
        }
    }
}
