using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Tests
{
    [TestClass]
    public class JobTests
    {
        private UiPathCloudAPI uiPath;
        private bool _isReady;

        [TestInitialize]
        public void Init()
        {
            uiPath = Config.CommonUiPathApi;
            _isReady = false;
        }

        [TestMethod]
        public void JobManipulationTest()
        {
            try
            {
                QueryParameters queryParameters = new QueryParameters
                {
                    Top = 1,
                    OrderBy = new OrderBy("CreationTime", SortDirection.Desc),
                    Filter = new Filter("State", JobState.Stopped),
                    Expand = "Robot,Release",
                    Select = "Robot,Release"
                };
                var job = uiPath.JobManager.GetCollection(queryParameters).FirstOrDefault();
                if (job != null)
                {
                    uiPath.JobManager.WaitReadyJobCompleted += JobManager_WaitReadyJobCompleted;
                    var newJob = uiPath.JobManager.StartJob(job.Robot, job.Release);
                    Assert.IsNotNull(newJob);
                    WaitReadyJobAsync(newJob);
                    Assert.AreEqual(newJob.State, JobState.Pending);
                    uiPath.JobManager.StopJob(newJob);
                    Thread.Sleep(10000);
                    newJob = uiPath.JobManager.GetInstance(newJob);
                    Assert.AreEqual(newJob.State, JobState.Stopped);
                    Assert.IsTrue(_isReady);
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

        private void JobManager_WaitReadyJobCompleted(object sender, Managers.WaitReadyJobCompletedEventArgs e)
        {
            _isReady = true;
        }

        private async void WaitReadyJobAsync(Job job)
        {
            await uiPath.JobManager.WaitReadyJobAsync(job);
        }
    }
}
