using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Tests
{
    [TestClass]
    public class RetrievingDataTests
    {
        private UiPathCloudAPI uiPath;
        private Folder TestFolderOrigin;
        private Folder TestFolder;
        private Folder TestFolderFail;
        private TestContext testContextInstance;

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestInitialize]
        public void Init()
        {
            uiPath = Config.CommonUiPathApi;
            TestFolderOrigin = uiPath.DefaultFolder;
            TestFolder = uiPath.FolderManager.GetInstance("Default");
            TestFolderFail = new Folder
            {
                DisplayName = "Lol lel kek",
                Id = int.MaxValue
            };
        }

        [TestMethod]
        public void GetRobotsTest()
        {
            uiPath.RobotManager.UseSession = false;
            var robots = uiPath.RobotManager.GetCollection();
            Assert.IsNotNull(robots);
            int count = uiPath.RobotManager.Count();
            Assert.AreEqual(robots.Count(), count);
            uiPath.RobotManager.UseSession = true;
            robots = uiPath.RobotManager.GetCollection();
            Assert.IsNotNull(robots);
            count = uiPath.RobotManager.Count();
            Assert.AreEqual(robots.Count(), count);
            if (robots.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Type"));
                queryParameters.Filter = new Filter("Type", robots.First().Type);
                robots = uiPath.RobotManager.GetCollection(queryParameters);
                Assert.IsTrue(robots.Count() == 1);
                var robot = uiPath.RobotManager.GetInstance(robots.First());
                Assert.IsNotNull(robot);
                Assert.AreEqual(robot.Name, robots.First().Name);
                if (count > 1)
                {
                    queryParameters = new QueryParameters(skip: 1);
                    robots = uiPath.RobotManager.GetCollection(queryParameters);
                    Assert.AreEqual(robots.Count(), count - 1);
                }
            }
        }

        [TestMethod]
        public void GetRobotsInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetRobotsTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetRobotsTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetProcessesTest()
        {
            var processes = uiPath.ProcessManager.GetCollection();
            Assert.IsNotNull(processes);
            int count = uiPath.ProcessManager.Count();
            Assert.AreEqual(processes.Count(), count);
            if (processes.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Name"));
                queryParameters.Filter = new Filter("Key", processes.First().Key);
                processes = uiPath.ProcessManager.GetCollection(queryParameters);
                Assert.IsTrue(processes.Count() == 1);
                var process = uiPath.ProcessManager.GetInstance(processes.First());
                Assert.IsNotNull(process);
                Assert.AreEqual(process.Name, processes.First().Name);
                if (count > 1)
                {
                    queryParameters = new QueryParameters(skip: 1);
                    processes = uiPath.ProcessManager.GetCollection(queryParameters);
                    Assert.AreEqual(processes.Count(), count - 1);
                }
            }
        }

        [TestMethod]
        public void GetProcessesInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetProcessesTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetProcessesTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetJobsTest()
        {
            var jobs = uiPath.JobManager.GetCollection();
            Assert.IsNotNull(jobs);
            int count = uiPath.JobManager.Count();
            Assert.AreEqual(jobs.Count(), count);
            if (jobs.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("CreationTime", SortDirection.Desc));
                queryParameters.Filter = new Filter("State", jobs.First().State);
                jobs = uiPath.JobManager.GetCollection(queryParameters);
                Assert.IsTrue(jobs.Count() == 1);
                var job = uiPath.JobManager.GetInstance(jobs.First());
                Assert.IsNotNull(job);
                Assert.AreEqual(job.CreationTime, jobs.First().CreationTime);
                if (count > 1)
                {
                    queryParameters = new QueryParameters(skip: 1);
                    jobs = uiPath.JobManager.GetCollection(queryParameters);
                    Assert.AreEqual(jobs.Count(), count - 1);
                }
                if (count > 2 && uiPath.JobManager.GetCollection().GroupBy(j => j.State).Count() > 1)
                {
                    var jobs1 = uiPath.JobManager.GetCollection(top: 2, orderby: new OrderBy("State", SortDirection.Asc));
                    var jobs2 = uiPath.JobManager.GetCollection(top: 2, orderby: new OrderBy("State", SortDirection.Asc), skip: 1);
                    var jobs3 = uiPath.JobManager.GetCollection(top: 2, orderby: new OrderBy("State", SortDirection.Desc));
                    var jobs4 = uiPath.JobManager.GetCollection(top: 2, orderby: new OrderBy("State", SortDirection.Desc), skip: 1);
                    Assert.AreEqual(jobs1.ElementAt(1).Id, jobs2.ElementAt(0).Id);
                    Assert.AreNotEqual(jobs1.ElementAt(0).Id, jobs2.ElementAt(0).Id);
                    Assert.AreEqual(jobs3.ElementAt(1).Id, jobs4.ElementAt(0).Id);
                }
            }
        }

        [TestMethod]
        public void GetJobsInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetJobsTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetJobsTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetEnvironmentsTest()
        {
            var environments = uiPath.EnvironmentManager.GetCollection();
            Assert.IsNotNull(environments);
            int count = uiPath.EnvironmentManager.Count();
            Assert.AreEqual(environments.Count(), count);
            if (environments.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Name"));
                queryParameters.Filter = new Filter("Name", environments.First().Name);
                environments = uiPath.EnvironmentManager.GetCollection(queryParameters);
                Assert.IsTrue(environments.Count() == 1);
                var environment = uiPath.EnvironmentManager.GetInstance(environments.First());
                Assert.IsNotNull(environment);
                Assert.AreEqual(environment.Name, environments.First().Name);
                if (count > 1)
                {
                    queryParameters = new QueryParameters(skip: 1);
                    environments = uiPath.EnvironmentManager.GetCollection(queryParameters);
                    Assert.AreEqual(environments.Count(), count - 1);
                }
            }
        }

        [TestMethod]
        public void GetEnvironmentsInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetEnvironmentsTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetEnvironmentsTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetMachinesTest()
        {
            var machines = uiPath.MachineManager.GetCollection();
            Assert.IsNotNull(machines);
            int count = uiPath.MachineManager.Count();
            Assert.AreEqual(machines.Count(), count);
            if (machines.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Name"));
                queryParameters.Filter = new Filter("Type", machines.First().Type);
                machines = uiPath.MachineManager.GetCollection(queryParameters);
                Assert.IsTrue(machines.Count() == 1);
                var machine = uiPath.MachineManager.GetInstance(machines.First());
                Assert.IsNotNull(machine);
                Assert.AreEqual(machine.Name, machines.First().Name);
                if (count > 1)
                {
                    queryParameters = new QueryParameters(skip: 1);
                    machines = uiPath.MachineManager.GetCollection(queryParameters);
                    Assert.AreEqual(machines.Count(), count - 1);
                }
            }
        }

        [TestMethod]
        public void GetMachinesInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetMachinesTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetMachinesTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetAssetsTest()
        {
            var assets = uiPath.AssetManager.GetCollection();
            Assert.IsNotNull(assets);
            int count = uiPath.AssetManager.Count();
            Assert.AreEqual(assets.Count(), count);
            if (assets.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Name"));
                queryParameters.Filter = new Filter("Name", assets.First().Name);
                assets = uiPath.AssetManager.GetCollection(queryParameters);
                Assert.IsTrue(assets.Count() == 1);
                if (count > 1)
                {
                    queryParameters = new QueryParameters(skip: 1);
                    assets = uiPath.AssetManager.GetCollection(queryParameters);
                    Assert.AreEqual(assets.Count(), count - 1);
                }
            }
        }

        [TestMethod]
        public void GetAssetsInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetAssetsTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetAssetsTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetRobotsForProcessTest()
        {
            var processes = uiPath.ProcessManager.GetCollection();
            foreach (var process in processes)
            {
                var robots = uiPath.RobotManager.GetRobotsForProcess(process);
            }
        }

        [TestMethod]
        public void GetRobotsForProcessInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetRobotsForProcessTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetRobotsForProcessTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetRobotLogsTest()
        {
            var logs = uiPath.RobotManager.GetLogs();
            Assert.IsNotNull(logs);
            int count = uiPath.RobotManager.LogCount();
            var robots = uiPath.RobotManager.GetCollection();
            foreach (var robot in robots)
            {
                int countForRobot = uiPath.RobotManager.LogCount(robot);
                if (countForRobot <= 1000)
                {
                    var logsForRobot = uiPath.RobotManager.GetLogs(robot);
                    Assert.AreEqual(logsForRobot.Count(), countForRobot);
                }
            }
        }

        [TestMethod]
        public void GetRobotLogsInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetRobotLogsTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetRobotLogsTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetQueueItemsTest()
        {
            var queueItems = uiPath.TransactionManager.GetCollection();
            Assert.IsNotNull(queueItems);
            int count = uiPath.TransactionManager.Count();
            Assert.AreEqual(queueItems.Count(), count);
            if (queueItems.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Status"));
                queryParameters.Filter = new Filter("Status", queueItems.First().Status);
                queueItems = uiPath.TransactionManager.GetCollection(queryParameters);
                Assert.IsTrue(queueItems.Count() == 1);
                var queueItem = uiPath.TransactionManager.GetInstance(queueItems.First());
                Assert.IsNotNull(queueItem);
                Assert.AreEqual(queueItem.Id, queueItems.First().Id);
                if (count > 1)
                {
                    queryParameters = new QueryParameters(skip: 1);
                    queueItems = uiPath.TransactionManager.GetCollection(queryParameters);
                    Assert.AreEqual(queueItems.Count(), count - 1);
                }
            }
        }

        [TestMethod]
        public void GetQueueInFolderItemsTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetQueueItemsTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetQueueItemsTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }

        [TestMethod]
        public void GetQueueItemEventsTest()
        {
            var queueItemEvents = uiPath.TransactionManager.GetQueueItemEvents();
            Assert.IsNotNull(queueItemEvents);
            int count = uiPath.TransactionManager.QueueItemEventCount();
            if (queueItemEvents.Any())
            {
                QueryParameters queryParameters = new QueryParameters(top: 1, orderby: new OrderBy("Status"));
                queryParameters.Filter = new Filter("Status", queueItemEvents.First().Status);
                queueItemEvents = uiPath.TransactionManager.GetQueueItemEvents(queryParameters);
                Assert.IsTrue(queueItemEvents.Count() == 1);
                var queueItemEvent = uiPath.TransactionManager.GetQueueItemEvent(queueItemEvents.First());
                Assert.IsNotNull(queueItemEvent);
                Assert.AreEqual(queueItemEvent.Id, queueItemEvents.First().Id);
                var queueItem = uiPath.TransactionManager.GetCollection(top: 1).FirstOrDefault();
                if (queueItem != null)
                {
                    queueItemEvents = uiPath.TransactionManager.GetQueueItemEventsHistory(queueItem);
                }
            }
        }

        [TestMethod]
        public void GetQueueItemEventsInFolderTest()
        {
            var originFolder = uiPath.DefaultFolder;
            uiPath.DefaultFolder = TestFolder;
            GetQueueItemEventsTest();
            uiPath.DefaultFolder = TestFolderFail;
            try
            {
                GetQueueItemEventsTest();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    TestContext.WriteLine("UiPath return error: {0}", uiPath.LastErrorMessage);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                uiPath.DefaultFolder = originFolder;
            }
        }
    }
}
