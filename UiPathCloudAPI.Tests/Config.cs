using System;
using Microsoft.Extensions.Configuration;

namespace UiPathCloudAPISharp.Tests
{
    internal class Config
    {
        private static IConfiguration _configuration;
        private static UiPathCloudAPI _uiPath;

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }

        internal static UiPathCloudAPI CommonUiPathApi
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = Config.InitConfiguration();
                }
                if (_uiPath == null)
                {
                    _uiPath = new UiPathCloudAPI();
                    _uiPath.Initialization(_configuration["TenantLogicalName"], _configuration["ClientId"], _configuration["UserKey"], BehaviorMode.AutoAuthorization);
                }
                return _uiPath;
            }
        }
    }
}