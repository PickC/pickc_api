using appify.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache cache;
        public AppifyTokenController(IConfiguration config, IDistributedCache cache)
        {
            this.config = config;
            this.cache = cache;
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
                ////new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("userID",item.UserID.ToString()),
                new Claim("deviceID",item.DeviceID.ToString()),

            };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer:config["Jwt:Issuer"],
                    audience:config["Jwt.Audience"],
                    claims:claims,
                    //expires: DateTime.Now.AddDays(7),
                    expires: DateTime.Now.AddMinutes(10),   // the expiry date/time is updated to 10 minutes for testing purpose.
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

        [HttpPost, Route("generatetoken")]
        public async Task<string> GetOrGenerateToken(TokenObject item)
        {
            var cacheKey = $"Token_{item.UserID}";
            var existingToken = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(existingToken))
            {
                return existingToken; // Return existing valid token
            }

            var newToken = GenerateJwtToken(item);
            await cache.SetStringAsync(cacheKey, newToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });

            return newToken;
        }

        private string GenerateJwtToken(TokenObject item)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, item.UserID.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, item.DeviceID.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt.Audience"],
                claims: claims,
                ////expires: DateTime.UtcNow.AddDays(7), // 7-day expiration
                expires: DateTime.Now.AddMinutes(10), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
