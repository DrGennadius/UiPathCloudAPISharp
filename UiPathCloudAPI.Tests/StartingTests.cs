using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using UiPathCloudAPISharp;

namespace UiPathCloudAPISharp.Tests
{
    [TestClass]
    public class StartingTests
    {
        private IConfiguration _configuration;

        [TestInitialize]
        public void Init()
        {
            _configuration = Config.InitConfiguration();
        }

        [TestMethod]
        public void InitTest()
        {
            UiPathCloudAPI uiPath1 = new UiPathCloudAPI();
            uiPath1.Initialization(_configuration["TenantLogicalName"], _configuration["ClientId"], _configuration["UserKey"]);
            Assert.IsFalse(uiPath1.IsAuthorized);
            uiPath1.Authorization();
            Assert.IsTrue(uiPath1.IsAuthorized);

            UiPathCloudAPI uiPath2 = new UiPathCloudAPI(_configuration["TenantLogicalName"], _configuration["ClientId"], _configuration["UserKey"]);
            Assert.IsFalse(uiPath2.IsAuthorized);
            uiPath2.Authorization();
            Assert.IsTrue(uiPath2.IsAuthorized);

            UiPathCloudAPI uiPath3 = new UiPathCloudAPI(_configuration["TenantLogicalName"], _configuration["ClientId"], _configuration["UserKey"], BehaviorMode.AutoInitiation);
            var robots = uiPath3.RobotManager.GetCollection();
            Assert.IsNotNull(robots);
        }
    }
}
