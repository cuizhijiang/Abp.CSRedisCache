﻿using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching.CSRedis
{
    /// <summary>
    ///     Used to create <see cref="AbpRedisCache" /> instances.
    /// </summary>
    public class AbpRedisCacheManager : CacheManagerBase<AbpRedisCache>
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbpRedisCacheManager" /> class.
        /// </summary>
        public AbpRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<AbpRedisCache>(DependencyLifeStyle.Transient);
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches)
            {
                _iocManager.Release(cache.Value);
            }
        }

        protected override AbpRedisCache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<AbpRedisCache>(new {name});
        }
    }
}