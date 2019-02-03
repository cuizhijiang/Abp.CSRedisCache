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
            if (options.DatabaseId > -1)
            {
                options.ConnectionString = options.ConnectionString.TrimEnd(';') + ",defaultDatabase=" + options.DatabaseId;
            }
            RedisHelper.Initialization(new CSRedis.CSRedisClient(options.ConnectionString));
        }
    }
}
