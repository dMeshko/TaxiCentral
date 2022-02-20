using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TaxiCentral.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration 
                             ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost]
        public ActionResult<string> Authenticate(string pin)
        {
            // validate user
            var user = ValidateUserCredentials(pin);
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
                new("family_name", user.Surname),
                new("city", user.City)
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

        private User? ValidateUserCredentials(string pin)
        {
            if (string.IsNullOrEmpty(pin))
            {
                return null;
            }

            //todo: check against db
            return new User()
            {
                Id = Guid.NewGuid(),
                Name = "Darko",
                Surname = "Meshkovski",
                City = "Bitola"
            };
        }

        private class User
        {
           public Guid Id { get; set; }
           public string Name { get; set; }
           public string Surname { get; set; }
           public string City { get; set; }
        }
    }
}
