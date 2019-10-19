using System.Configuration;
using Abp.Configuration.Startup;
using Abp.Extensions;

namespace Abp.Runtime.Caching.CSRedis
{
    public class AbpRedisCacheOptions
    {
        private const string ConnectionStringKey = "Abp.Redis.Cache";

        private const string DatabaseIdSettingKey = "Abp.Redis.Cache.DatabaseId";

        public AbpRedisCacheOptions(IAbpStartupConfiguration abpStartupConfiguration)
        {
            AbpStartupConfiguration = abpStartupConfiguration;

            ConnectionString = GetDefaultConnectionString();
            DatabaseId = GetDefaultDatabaseId();
        }

        public IAbpStartupConfiguration AbpStartupConfiguration { get; }

        public string ConnectionString { get; set; }

        public int DatabaseId { get; set; }

        private static int GetDefaultDatabaseId()
        {
            var appSetting = ConfigurationManager.AppSettings[DatabaseIdSettingKey];
            if (appSetting.IsNullOrEmpty()) return -1;

            int databaseId;
            if (!int.TryParse(appSetting, out databaseId)) return -1;

            return databaseId;
        }

        private static string GetDefaultConnectionString()
        {
            var connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            if (connStr == null || connStr.ConnectionString.IsNullOrWhiteSpace()) return "localhost";

            return connStr.ConnectionString;
        }
    }
}