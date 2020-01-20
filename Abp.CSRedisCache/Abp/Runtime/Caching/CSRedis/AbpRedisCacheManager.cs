using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching.CSRedis
{
    /// <summary>
    ///     Used to create <see cref="AbpRedisCache" /> instances.
    /// </summary>
    public class AbpRedisCacheManager : CacheManagerBase<AbpRedisCache>,ICacheManager
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


        /// <summary>Gets all caches.</summary>
        /// <returns>List of caches</returns>
        public new IReadOnlyList<ICache> GetAllCaches()
        {
            return base.GetAllCaches();
        }

        /// <summary>
        /// Gets a <see cref="T:Abp.Runtime.Caching.ICache" /> instance.
        /// It may create the cache if it does not already exists.
        /// </summary>
        /// <param name="name">Unique and case sensitive name of the cache.</param>
        /// <returns>The cache reference</returns>
        public new ICache GetCache(string name)
        {
            return base.GetCache(name);
        }
    }
}