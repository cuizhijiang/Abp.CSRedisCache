﻿using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching.CSRedis
{
    /// <summary>
    ///     Used to create <see cref="AbpRedisCache" /> instances.
    /// </summary>
    public class AbpRedisCacheManager : CacheManagerBase
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
            _iocManager.Dispose();
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<AbpRedisCache>(new {name});
        }
    }
}