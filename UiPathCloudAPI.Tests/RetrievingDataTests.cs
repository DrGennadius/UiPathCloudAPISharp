using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
        public void GetJobsTest()
        {
            try
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
        public void GetEnvironmentsTest()
        {
            try
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
        public void GetMachinesTest()
        {
            try
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
