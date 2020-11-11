﻿using System;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;
using CSRedis;

namespace Abp.Runtime.Caching.CSRedis
{
    /// <summary>
    ///     Extension methods for <see cref="ICachingConfiguration" />.
    /// </summary>
    public static class RedisCacheConfigurationExtensions
    {
        /// <summary>
        ///     Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        public static void UseCSRedis(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.UseCSRedis(options => { });
        }

        /// <summary>
        ///     Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseCSRedis(this ICachingConfiguration cachingConfiguration,
            Action<AbpRedisCacheOptions> optionsAction)
        {
            var iocManager = cachingConfiguration.AbpConfiguration.IocManager;

            iocManager.RegisterIfNot<ICacheManager, AbpRedisCacheManager>();

            var options= iocManager.Resolve<AbpRedisCacheOptions>();
            optionsAction(options);

            var connectionString = options.ConnectionString;
            if (options.DatabaseId > -1)
                connectionString = options.ConnectionString.TrimEnd(';') + ",defaultDatabase=" + options.DatabaseId;
            RedisHelper.Initialization(new CSRedisClient(connectionString));
        }
    }
}