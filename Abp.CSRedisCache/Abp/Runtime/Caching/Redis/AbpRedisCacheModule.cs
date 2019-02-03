using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// This modules is used to replace ABP's cache system with Redis server.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpRedisCacheModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpRedisCacheOptions>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpRedisCacheModule).GetAssembly());
        }
        public override void PostInitialize()
        {
            var options = IocManager.Resolve<AbpRedisCacheOptions>();
            var connectionString = options.ConnectionString;
            if (options.DatabaseId > -1)
            {
                connectionString = options.ConnectionString.TrimEnd(';') + ",defaultDatabase=" + options.DatabaseId;
            }
            RedisHelper.Initialization(new CSRedis.CSRedisClient(connectionString));
        }
    }
}
