﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UiPathCloudAPISharp.Common;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Managers
{
    public class JobManager : IManager, IGetRequest<JobWithArguments>
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestExecutor _requestExecutor;

        private RobotManager _robotManager;

        private ProcessManager _processManager;

        internal JobManager(RequestExecutor requestExecutor, RobotManager robotManager, ProcessManager processManager)
        {
            _requestExecutor = requestExecutor;
            _robotManager = robotManager;
            _processManager = processManager;
        }

        /// <summary>
        /// Completed Event Handler for waitting ready Job.
        /// </summary>
        public event WaitReadyJobCompletedEventHandler WaitReadyJobCompleted;

        public IEnumerable<JobWithArguments> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Jobs");
            return JsonConvert.DeserializeObject<Info<JobWithArguments>>(response).Items;
        }

        public IEnumerable<JobWithArguments> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Jobs", folder);
            return JsonConvert.DeserializeObject<Info<JobWithArguments>>(response).Items;
        }

        public IEnumerable<JobWithArguments> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<JobWithArguments> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<JobWithArguments> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Jobs", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<JobWithArguments>>(response).Items;
        }

        public JobWithArguments GetInstance(int id, Folder folder = null)
        {
            return GetCollection(new Filter("Id", id), folder).FirstOrDefault();
        }

        public JobWithArguments GetInstance(JobWithArguments instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public JobWithArguments GetInstance(Job instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        /// <summary>
        /// Start new job by robot and proccess release.
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public Job StartJob(Robot robot, Process process, Folder folder = null)
        {
            return StartJob(robot.Id, process.Key, folder);
        }

        /// <summary>
        /// Start new job by robot and proccess release.
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="process"></param>
        /// <param name="inputArguments">Input Arguments</param>
        /// <returns></returns>
        public Job StartJob(Robot robot, Process process, Dictionary<string, object> inputArguments, Folder folder = null)
        {
            return StartJob(robot.Id, process.Key, inputArguments, folder);
        }

        /// <summary>
        /// Start new job by robot name and proccess name.
        /// <para>(!) Experimental</para>
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="processKey"></param>
        /// <param name="environmentName"></param>
        /// <returns></returns>
        public Job StartJob(string robotName, string processKey, string environmentName, Folder folder = null)
        {
            return StartJob(robotName, processKey + "_" + environmentName, folder);
        }

        /// <summary>
        /// Start new job by robot name and proccess name.
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public Job StartJob(string robotName, string processName, Folder folder = null)
        {
            return StartJob(robotName, processName, new Dictionary<string, object>(), folder);
        }

        /// <summary>
        /// Start new job by robot name and proccess name.
        /// <para>(!) Experimental</para>
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="processKey"></param>
        /// <param name="environmentName"></param>
        /// <param name="inputArguments"></param>
        /// <returns></returns>
        public Job StartJob(string robotName, string processKey, string environmentName, Dictionary<string, object> inputArguments, Folder folder = null)
        {
            return StartJob(robotName, processKey + "_" + environmentName, inputArguments, folder);
        }

        /// <summary>
        /// Start new job by robot name and proccess name.
        /// </summary>
        /// <param name="robotName">Robot Name</param>
        /// <param name="processName">{Process Key} + _ + {Environment Name}</param>
        /// <param name="inputArguments">Input Arguments</param>
        /// <returns></returns>
        public Job StartJob(string robotName, string processName, Dictionary<string, object> inputArguments, Folder folder = null)
        {
            Robot robot = null;
            if (_robotManager.UseSession)
            {
                // TODO: Fix it
                robot = _robotManager.GetCollection(new Filter("Robot/Name", robotName), folder).FirstOrDefault();
            }
            else
            {
                robot = _robotManager.GetCollection(new Filter("Name", robotName), folder).FirstOrDefault();
            }
            if (robot == null)
            {
                throw new Exception("The specified robot was not found.");
            }
            Process process = _processManager.GetCollection(new Filter("Name", processName), folder).FirstOrDefault();
            if (process == null)
            {
                throw new Exception("The specified proccess was not found.");
            }
            return StartJob(robot.Id, process.Key, inputArguments, folder);
        }

        /// <summary>
        /// Start new job by robot id and proccess release key.
        /// </summary>
        /// <param name="robotId">Robot ID</param>
        /// <param name="releaseKey">Proccess release key</param>
        /// <returns></returns>
        public Job StartJob(int robotId, string releaseKey, Folder folder = null)
        {
            return StartJob(robotId, releaseKey, new Dictionary<string, object>(), folder);
        }

        /// <summary>
        /// Start new job by robot id and proccess release key.
        /// </summary>
        /// <param name="robotId">Robot ID</param>
        /// <param name="releaseKey">Proccess release key</param>
        /// <param name="inputArguments">Input Arguments</param>
        /// <returns></returns>
        public Job StartJob(int robotId, string releaseKey, Dictionary<string, object> inputArguments, Folder folder = null)
        {
            Job job = null;
            if (string.IsNullOrEmpty(releaseKey))
            {
                throw new ArgumentException("Proccess release key is empty.");
            }
            else
            {
                string output = "";
                if (inputArguments.Any())
                {
                    var startInfo = new StartInfoContainer<StartJobsInfoWithArguments>
                    {
                        StartJobsInfo = new StartJobsInfoWithArguments
                        {
                            ReleaseKey = releaseKey,
                            Strategy = "Specific",
                            RobotIds = new int[] { robotId },
                            InputArgumentsAsString = JsonConvert.SerializeObject(inputArguments)
                        }
                    };
                    output = JsonConvert.SerializeObject(startInfo);
                }
                else
                {
                    var startInfo = new StartInfoContainer<StartJobsInfo>
                    {
                        StartJobsInfo = new StartJobsInfo
                        {
                            ReleaseKey = releaseKey,
                            Strategy = "Specific",
                            RobotIds = new int[] { robotId }
                        }
                    };
                    output = JsonConvert.SerializeObject(startInfo);
                }
                //SentDataStore.Enqueue(output);
                byte[] sentData = Encoding.UTF8.GetBytes(output);
                string returnStr = null;
                try
                {
                    returnStr = _requestExecutor.SendRequestPostForOdata("Jobs/UiPath.Server.Configuration.OData.StartJobs", sentData, folder);
                    job = JsonConvert.DeserializeObject<Info<Job>>(returnStr).Items.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    string errorString = _requestExecutor.LastErrorMessage;
                    errorString += "\nException message:\n" + ex.Message;
                    if (!string.IsNullOrEmpty(returnStr))
                    {
                        errorString += "\nReturn string:\n" + returnStr;
                    }
                    throw new Exception(errorString);
                }
            }
            return job;
        }

        /// <summary>
        /// Stop or kill the job.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="stopStrategy">Strategy: Kill or SoftStop</param>
        public void StopJob(Job job, StopJobsStrategy stopStrategy = StopJobsStrategy.SoftStop, Folder folder = null)
        {
            StopJobs(new List<Job> { job }, stopStrategy, folder);
        }

        /// <summary>
        /// Stop or kill the jobs.
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="stopStrategy">Strategy: Kill or SoftStop</param>
        public void StopJobs(List<Job> jobs, StopJobsStrategy stopStrategy = StopJobsStrategy.SoftStop, Folder folder = null)
        {
            var startJobsInfo = new StopJobsInfo
            {
                Strategy = stopStrategy,
                JobIds = jobs.Select(j => j.Id).ToArray()
            };
            string output = JsonConvert.SerializeObject(startJobsInfo);
            //SentDataStore.Enqueue(output);
            byte[] sentData = Encoding.UTF8.GetBytes(output);
            _requestExecutor.SendRequestPostForOdata("Jobs/UiPath.Server.Configuration.OData.StopJobs", sentData, folder);
        }

        /// <summary>
        /// Async wait ready job.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<JobWithArguments> WaitReadyJobAsync(Job job, Folder folder = null)
        {
            return await WaitReadyJobAsync(job, _requestExecutor.WaitTimeout, folder);
        }

        /// <summary>
        /// Async wait ready big job.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<JobWithArguments> WaitReadyBigJobAsync(Job job, Folder folder = null)
        {
            return await WaitReadyJobAsync(job, _requestExecutor.BigWaitTimeout, folder);
        }

        /// <summary>
        /// Async wait ready job with timeout.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<JobWithArguments> WaitReadyJobAsync(Job job, int timeout, Folder folder = null)
        {
            JobWithArguments readyJob = await Task.Factory.StartNew<JobWithArguments>(() => WaitReadyJob(job, timeout, folder));
            OnWaitReadyJobCompleted(new WaitReadyJobCompletedEventArgs() { ReadyJob = readyJob });
            return readyJob;
        }

        /// <summary>
        /// Wait ready job.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public JobWithArguments WaitReadyJob(Job job, Folder folder = null)
        {
            return WaitReadyJob(job, _requestExecutor.WaitTimeout, folder);
        }

        /// <summary>
        /// Wait ready big job.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public JobWithArguments WaitReadyBigJob(Job job, Folder folder = null)
        {
            return WaitReadyJob(job, _requestExecutor.BigWaitTimeout, folder);
        }

        /// <summary>
        /// Wait ready job with timeout.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public JobWithArguments WaitReadyJob(Job job, int timeout, Folder folder = null)
        {
            JobWithArguments readyJob = null;

            if (job == null)
            {
                throw new ArgumentException("Job is NULL.");
            }
            else
            {
                DateTime stopDateTime = DateTime.Now.AddMilliseconds(timeout);
                while (true)
                {
                    Thread.Sleep(5000);
                    var returnJob = GetInstance(job.Id, folder);
                    if (DateTime.Now >= stopDateTime)
                    {
                        break;
                    }
                    else if (
                        returnJob.State != JobState.Pending && 
                        returnJob.State != JobState.Running && 
                        returnJob.State != JobState.Stopping && 
                        returnJob.State != JobState.Terminating
                    )
                    {
                        readyJob = returnJob;
                        break;
                    }
                }
                if (readyJob == null)
                {
                    throw new Exception("Timeout!");
                }
            }

            return readyJob;
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Jobs", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<JobWithArguments>>(response).Count;
        }

        protected void OnWaitReadyJobCompleted(WaitReadyJobCompletedEventArgs e)
        {
            if (WaitReadyJobCompleted != null)
            {
                WaitReadyJobCompleted(this, e);
            }
        }
    }
}
