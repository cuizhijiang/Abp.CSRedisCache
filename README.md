# Abp.CSRedisCache
[![NuGet](https://img.shields.io/nuget/v/Abp.CSRedisCache.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Abp.CSRedisCache/)
## Getting Started
### Configuration
Add a DependsOn attribute for the AbpRedisCacheModule and call the UseRedis extension method in the PreInitialize method of your module, as shown below:
```csharp
[DependsOn(
    //...other module dependencies
    typeof(AbpRedisCacheModule))]
public class MyProjectWebModule : AbpModule
{
    public override void PreInitialize()
    {
        //...other configurations
        
        Configuration.Caching.UseRedis(options =>
        {
            options.ConnectionString = "127.0.0.1:6379,pass=123,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240";
        });
        
    }
    
    //...other code
}
