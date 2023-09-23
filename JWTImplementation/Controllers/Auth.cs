using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JWTImplementation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        IConfiguration _builder;
        public Auth(IConfiguration builder) { _builder = builder; }
       

        [HttpGet("GetToken")]
        // Method to get the token
        public IActionResult GetToken() {
            try
            {
                var config = _builder;
                var ValidIssuer = config["JWTSettings:Issuer"];
                var ValidAudience = config["JWTSettings:Audiance"];
                // Hardcoading here as  this has be kept in db,
                // we are restricting the scope of this tutorial to only token creation and validation

                string username = "User123";
                string password = "password@123";
                var IssuerSigningKey = Encoding.ASCII.GetBytes(config["JWTSettings:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                         new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,username ),
                new Claim(JwtRegisteredClaimNames.Email,username ),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    Issuer = ValidIssuer,
                    Audience = ValidAudience,
                    SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(IssuerSigningKey),
                SecurityAlgorithms.HmacSha512Signature),
                };
                var tokenHandeler = new JwtSecurityTokenHandler();
                var token = tokenHandeler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandeler.WriteToken(token);
                return Ok(jwtToken);
            }
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
