using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Reflection.Extensions;

namespace Abp.Runtime.Caching.CSRedis
{
    /// <summary>
    ///     Used to store cache in a Redis server.
    /// </summary>
    public class AbpRedisCache : CacheBase
    {
        private readonly IRedisCacheSerializer _serializer;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public AbpRedisCache(
            string name,
            IRedisCacheSerializer redisCacheSerializer)
            : base(name)
        {
            _serializer = redisCacheSerializer;
        }

        public override object GetOrDefault(string key)
        {
            var objbyte = RedisHelper.Get(GetLocalizedRedisKey(key));
            return !string.IsNullOrWhiteSpace(objbyte) ? Deserialize(objbyte) : null;
        }

        public override object[] GetOrDefault(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            var redisValues = RedisHelper.MGet(redisKeys.ToArray());
            var objbytes = redisValues.Select(obj => !string.IsNullOrWhiteSpace(obj) ? Deserialize(obj) : null);
            return objbytes.ToArray();
        }

        public override async Task<object> GetOrDefaultAsync(string key)
        {
            return await Task.FromResult(GetOrDefault(key));
        }

        public override async Task<object[]> GetOrDefaultAsync(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            var redisValues = await RedisHelper.MGetAsync(redisKeys.ToArray());
            var objbytes = redisValues.Select(obj => !string.IsNullOrWhiteSpace(obj) ? Deserialize(obj) : null);
            return objbytes.ToArray();
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null,
            DateTimeOffset? absoluteExpireTime = null)
        {
            if (value == null) throw new AbpException("Can not insert null values to the cache!");

            var absoluteExpireTimeSpan = absoluteExpireTime - DateTimeOffset.Now;
            var defaultAbsoluteExpireTimeTimeSpan = DefaultAbsoluteExpireTime - DateTimeOffset.Now;


            RedisHelper.Set(GetLocalizedRedisKey(key), Serialize(value, GetSerializableType(value)),
                absoluteExpireTimeSpan ?? slidingExpireTime ?? defaultAbsoluteExpireTimeTimeSpan ?? DefaultSlidingExpireTime);
        }

        public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null,
            DateTimeOffset? absoluteExpireTime = null)
        {
            if (value == null) throw new AbpException("Can not insert null values to the cache!");
            
            var absoluteExpireTimeSpan = absoluteExpireTime - DateTimeOffset.Now;
            var defaultAbsoluteExpireTimeTimeSpan = DefaultAbsoluteExpireTime - DateTimeOffset.Now;

            await RedisHelper.SetAsync(
                GetLocalizedRedisKey(key),
                Serialize(value, GetSerializableType(value)),
                absoluteExpireTimeSpan ?? slidingExpireTime ?? defaultAbsoluteExpireTimeTimeSpan ?? DefaultSlidingExpireTime
            );
        }

        public override void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null,
            DateTimeOffset? absoluteExpireTime = null)
        {
            if (pairs.Any(p => p.Value == null)) throw new AbpException("Can not insert null values to the cache!");

            var redisPairs = pairs.Select(p => new KeyValuePair<string, object>
                (GetLocalizedRedisKey(p.Key), Serialize(p.Value, GetSerializableType(p.Value)))
            );

            if (slidingExpireTime.HasValue || absoluteExpireTime.HasValue)
                Logger.WarnFormat("{0}/{1} is not supported for Redis bulk insert of key-value pairs",
                    nameof(slidingExpireTime), nameof(absoluteExpireTime));
            RedisHelper.MSet(redisPairs.ToArray());
        }

        public override async Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null,
            DateTimeOffset? absoluteExpireTime = null)
        {
            if (pairs.Any(p => p.Value == null)) throw new AbpException("Can not insert null values to the cache!");

            var redisPairs = pairs.Select(p => new KeyValuePair<string, object>
                (GetLocalizedRedisKey(p.Key), Serialize(p.Value, GetSerializableType(p.Value)))
            );
            if (slidingExpireTime.HasValue || absoluteExpireTime.HasValue)
                Logger.WarnFormat("{0}/{1} is not supported for Redis bulk insert of key-value pairs",
                    nameof(slidingExpireTime), nameof(absoluteExpireTime));
            await RedisHelper.MSetAsync(redisPairs.ToArray());
        }

        public override void Remove(string key)
        {
            RedisHelper.Del(GetLocalizedRedisKey(key));
        }

        public override async Task RemoveAsync(string key)
        {
            await RedisHelper.DelAsync(GetLocalizedRedisKey(key));
        }

        public override void Remove(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            RedisHelper.Del(redisKeys.ToArray());
        }

        public override async Task RemoveAsync(string[] keys)
        {
            var redisKeys = keys.Select(GetLocalizedRedisKey);
            await RedisHelper.DelAsync(redisKeys.ToArray());
        }

        public override void Clear()
        {
            RedisHelper.Eval(@"
                local keys = redis.call('keys', ARGV[1]) 
                for i=1,#keys,5000 do 
                redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
                end", "", GetLocalizedRedisKey("*"));
        }

        protected virtual Type GetSerializableType(object value)
        {
            //TODO: This is a workaround for serialization problems of entities.
            //TODO: Normally, entities should not be stored in the cache, but currently Abp.Zero packages does it. It will be fixed in the future.
            var type = value.GetType();
            if (EntityHelper.IsEntity(type) && type.GetAssembly().FullName.Contains("EntityFrameworkDynamicProxies"))
                type = type.GetTypeInfo().BaseType;
            return type;
        }

        protected virtual string Serialize(object value, Type type)
        {
            return _serializer.Serialize(value, type);
        }

        protected virtual object Deserialize(string objbyte)
        {
            return _serializer.Deserialize(objbyte);
        }

        protected virtual string GetLocalizedRedisKey(string key)
        {
            return GetLocalizedKey(key);
        }

        [Obsolete]
        protected virtual string GetLocalizedKey(string key)
        {
            return "n:" + Name + ",c:" + key;
        }

        public override bool TryGetValue(string key, out object value)
        {
            try
            {
                value = RedisHelper.Get(GetLocalizedRedisKey(key));
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}