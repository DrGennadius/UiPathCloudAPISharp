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
        private UiPathCloudAPI uiPath;

        [TestInitialize]
        public void Init()
        {
            uiPath = Config.CommonUiPathApi;
        }

        [TestMethod]
        public void GetRobotsTest()
        {
            try
            {
                uiPath.RobotManager.UseSession = false;
                var robots = uiPath.RobotManager.GetCollection();
                Assert.IsNotNull(robots);
                uiPath.RobotManager.UseSession = true;
                robots = uiPath.RobotManager.GetCollection();
                Assert.IsNotNull(robots);
                if (robots.Any())
                {
                    QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Type"));
                    queryParameters.Filter = new Filter("Type", robots.First().Type);
                    robots = uiPath.RobotManager.GetCollection(queryParameters);
                    Assert.IsTrue(robots.Count() == 1);
                    var robot = uiPath.RobotManager.GetInstance(robots.First());
                    Assert.IsNotNull(robot);
                    Assert.AreEqual(robot.Name, robots.First().Name);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    throw new Exception(uiPath.LastErrorMessage, ex);
                }
                throw ex;
            }
        }

        [TestMethod]
        public void GetProcessesTest()
        {
            try
            {
                var processes = uiPath.ProcessManager.GetCollection();
                Assert.IsNotNull(processes);
                if (processes.Any())
                {
                    QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Name"));
                    queryParameters.Filter = new Filter("Key", processes.First().Key);
                    processes = uiPath.ProcessManager.GetCollection(queryParameters);
                    Assert.IsTrue(processes.Count() == 1);
                    var process = uiPath.ProcessManager.GetInstance(processes.First());
                    Assert.IsNotNull(process);
                    Assert.AreEqual(process.Name, processes.First().Name);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    throw new Exception(uiPath.LastErrorMessage, ex);
                }
                throw ex;
            }
        }

        [TestMethod]
        public void GetAssetsTest()
        {
            try
            {
                var assets = uiPath.AssetManager.GetCollection();
                Assert.IsNotNull(assets);
                if (assets.Any())
                {
                    QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Name"));
                    queryParameters.Filter = new Filter("Name", assets.First().Name);
                    assets = uiPath.AssetManager.GetCollection(queryParameters);
                    Assert.IsTrue(assets.Count() == 1);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    throw new Exception(uiPath.LastErrorMessage, ex);
                }
                throw ex;
            }
        }
    }
}
