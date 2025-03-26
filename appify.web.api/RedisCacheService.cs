/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
namespace appify.web.api
{
    public class RedisCacheService
    {
        private readonly IDatabase _redisDb;
        public RedisCacheService(string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _redisDb = redis.GetDatabase();
        }

        // Save a token with an expiration time
        public void SaveToken(string customerId, string deviceID, string token, TimeSpan expiry)
        {
            var key = $"customer:{customerId}:deviceID:{deviceID}:token";
            _redisDb.StringSet(key, token, expiry);
        }

        // Retrieve a token
        public string GetToken(string customerId, string deviceID)
        {
            var key = $"customer:{customerId}:deviceID:{deviceID}:token";
            return _redisDb.StringGet(key);
        }

        // Check if a token exists
        public bool TokenExists(string customerId, string deviceID)
        {
            var key = $"customer:{customerId}:deviceID:{deviceID}:token";
            return _redisDb.KeyExists(key);
        }

        // Delete a token
        public bool DeleteToken(string customerId,string deviceID)
        {
            var key = $"customer:{customerId}:deviceID:{deviceID}:token";
            return _redisDb.KeyDelete(key);
        }
    }
    public static class CheckToken
    {

        public static void IsValidToken(HttpRequest request, IConfiguration config)
        {
            try
            {
                var checkurl = request.GetDisplayUrl().Contains("swagger");
                string redisConnectionString = config["AppifyCache:Server"];
                var redisCacheService = new RedisCacheService(redisConnectionString);
                string Token = string.Empty;
                string UserId = string.Empty;
                string DeviceId = string.Empty;
                request.Headers.TryGetValue("Authorization", out var token);
                request.Headers.TryGetValue("UserId", out var userId);
                request.Headers.TryGetValue("DeviceId", out var deviceId);
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(deviceId))
                {
                    //return BadRequest("Missing required headers: Token, UserId, or DeviceId.");
                    throw new Exception("Missing required headers: Token, UserId, or DeviceId.");
                }
                Token = token.ToString().Replace("Bearer ", "");
                UserId = userId.ToString();
                DeviceId = deviceId.ToString();
                string tokenRedis = redisCacheService.GetToken(UserId, DeviceId);

                if (Token != tokenRedis)
                {
                    //return BadRequest("Invalid Token.");
                    throw new Exception("Invalid Token.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}