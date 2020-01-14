using Microsoft.Extensions.Configuration;

namespace UiPathCloudAPISharp.Tests
{
    internal class Config
    {
        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }
    }
}