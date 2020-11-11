using Abp.Modules;
using Abp.Reflection.Extensions;
using CSRedis;

namespace Abp.Runtime.Caching.CSRedis
{
    /// <summary>
    ///     This modules is used to replace ABP's cache system with Redis server.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpCSRedisCacheModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpRedisCacheOptions>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpCSRedisCacheModule).GetAssembly());
        }
    }
}