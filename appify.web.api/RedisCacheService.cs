/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
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
}
