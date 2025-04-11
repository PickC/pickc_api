using appify.models;
using appify.utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Razorpay.Api;
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
        private ResponseMessage rm;
        public AppifyTokenController(IConfiguration config, IDistributedCache cache)
        {
            this.config = config;
            this.cache = cache;
        }

        [HttpPost, Route("getuser")]
        [Authorize(Roles ="1000,1001")]
        public IActionResult GetUser(TokenObject item)
        {
            rm = new ResponseMessage();
            try
            {
                CheckToken.IsValidToken(Request, config);
                //TokenValidator.IsValidToken(Request, config, env);

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            //return Ok("Token validation successful. You may proceed to use this API endpoint.");
            return Ok(rm);
        }

        [HttpPost, Route("generatetoken")]
        public IActionResult GenerateJwtTokenlogic(TokenObject item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                // Redis connection string (replace with your Redis server details)
                string redisConnectionString = config["AppifyCache:Server"];//"localhost:5000";

                // JWT configuration
                string secretKey = config["Jwt:Key"];
                string issuer = config["Jwt:Issuer"];
                string audience = config["Jwt.Audience"];

                // Create instances of RedisCacheService and JwtTokenService
                var redisCacheService = new RedisCacheService(redisConnectionString);
                var jwtTokenService = new JwtTokenService(secretKey, issuer, audience);

                // Customer ID (for demonstration purposes)
                string customerId = item.UserID.ToString();
                string deviceID = item.DeviceID.ToString();
                // Check if the token exists in Redis
                string token = redisCacheService.GetToken(customerId, deviceID);

                if (string.IsNullOrEmpty(token) || !jwtTokenService.IsTokenValid(token))
                {
                    // Token is expired or doesn't exist, generate a new one
                    token = jwtTokenService.GenerateToken(customerId, deviceID, 7);
                    redisCacheService.SaveToken(customerId, deviceID, token, TimeSpan.FromDays(7)); ////TimeSpan.FromMinutes(2)
                    Console.WriteLine("New token generated and saved in Redis.");
                }
                else
                {
                    Console.WriteLine("Token is valid and retrieved from Redis.");
                }

                var result = ($"Token:{token}");
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "TOKEN HAS BEEN SUCCESSFULLY FETCHED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);
        }

        //[HttpPost, Route("gettoken")]
        //public IActionResult GetToken(TokenObject item)
        //{

        //    try
        //    {
        //        var claims = new[] {
        //        new Claim(JwtRegisteredClaimNames.Sub,config["Jwt:Subject"]),
        //        ////new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        //        new Claim("userID",item.UserID.ToString()),
        //        new Claim("deviceID",item.DeviceID.ToString()),

        //    };
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        //        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //        var token = new JwtSecurityToken(
        //            issuer:config["Jwt:Issuer"],
        //            audience:config["Jwt.Audience"],
        //            claims:claims,
        //            //expires: DateTime.Now.AddDays(7),
        //            expires: DateTime.Now.AddMinutes(10),   // the expiry date/time is updated to 10 minutes for testing purpose.
        //            signingCredentials: signIn
        //            );

        //        string tokenvalue = new JwtSecurityTokenHandler().WriteToken(token);
        //        return Ok(new { token = tokenvalue });

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
