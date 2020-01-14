using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Tests
{
    [TestClass]
    public class RetrievingDataTests
    {
        private IConfiguration _configuration;
        private UiPathCloudAPI uiPath;

        [TestInitialize]
        public void Init()
        {
            _configuration = Config.InitConfiguration();
            uiPath = new UiPathCloudAPI();
            uiPath.Initialization(_configuration["TenantLogicalName"], _configuration["ClientId"], _configuration["UserKey"], BehaviorMode.AutoAuthorization);
        }

        [TestMethod]
        public void GetRobotCollectionTest()
        {
            uiPath.RobotManager.UseSession = false;
            var robots = uiPath.RobotManager.GetCollection();
            Assert.IsNotNull(robots);
            uiPath.RobotManager.UseSession = true;
            robots = uiPath.RobotManager.GetCollection();
            Assert.IsNotNull(robots);
            if (robots.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1);
                queryParameters.Filter = new Filter("Type", robots.ElementAt(0).Type);
                robots = uiPath.RobotManager.GetCollection(queryParameters);
                Assert.IsTrue(robots.Count() == 1);

            }
        }

        [TestMethod]
        public void GetProcessCollectionTest()
        {
            var processes = uiPath.ProcessManager.GetCollection();
            Assert.IsNotNull(processes);
        }

        [TestMethod]
        public void GetAssetCollectionTest()
        {
            var assets = uiPath.AssetManager.GetCollection();
            Assert.IsNotNull(assets);
        }
    }
}
