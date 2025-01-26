using appify.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AppifyTokenController : ControllerBase
    {
        private readonly IConfiguration config;
        public AppifyTokenController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpPost, Route("getuser")]
        [Authorize]
        public IActionResult GetUser(TokenObject item)
        {

            return Ok(item);
        }

        [HttpPost, Route("gettoken")]
        public IActionResult GetToken(TokenObject item)
        {

            try
            {


                var claims = new[] {

                new Claim(JwtRegisteredClaimNames.Sub,config["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("userID",item.UserID),

            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer:config["Jwt:Issuer"],
                    audience:config["Jwt.Audience"],
                    claims:claims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: signIn
                    );

                string tokenvalue = new JwtSecurityTokenHandler().WriteToken(token);


                return Ok(new { token = tokenvalue });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
