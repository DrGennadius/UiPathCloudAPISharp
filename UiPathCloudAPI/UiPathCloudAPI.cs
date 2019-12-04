using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.OData;

namespace UiPathCloudAPISharp
{
    /// <summary>
    /// A class that provides access to Orchestrator operations via the Cloud API.
    /// </summary>
    public class UiPathCloudAPI
    {
        #region Public properties

        /// <summary>
        /// User Key for connect to UiPath Orchestrator via Cloud API.
        /// Used as refresh_token for Authorization.
        /// </summary>
        public string UserKey { get; set; }

        /// <summary>
        /// Client Id for connect to UiPath Orchestrator via Cloud API.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Tenant Logical Name for connect to UiPath Orchestrator via Cloud API.
        /// </summary>
        public string TenantLogicalName { get; private set; }

        /// <summary>
        /// Last error message that occurred
        /// </summary>
        public string LastErrorMessage { get; private set; }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for the <see cref="HttpWebRequest.GetResponse"/> 
        /// and <see cref="HttpWebRequest.GetRequestStream"/> methods.
        /// <para >The default value is 30,000 milliseconds (30 seconds).</para>
        /// </summary>
        public int RequestTimeout { get; set; } = 30000;

        /// <summary>
        /// Passed an authentication?
        /// </summary>
        public bool Authorized { get; private set; } = false;

        /// <summary>
        /// Store for sent JSON data.
        /// </summary>
        public Queue<string> SentDataStore { get; private set; }

        /// <summary>
        /// Last issue response (deserialized).
        /// </summary>
        public Response LastIssueResponse { get; private set; }

        /// <summary>
        /// Current target account.
        /// </summary>
        public Account TargetAccount { get; private set; }

        /// <summary>
        /// Current target service instance.
        /// </summary>
        public ServiceInstance TargetServiceInstance { get; private set; }

        #endregion Public fields

        #region Private and internal properties

        internal AuthToken Token { get; set; }

        private List<ServiceInstance> ServiceInstances { get; set; }

        private AccountsForUser TargetUser { get; set; }        

        private readonly string urlUipathAuth = "https://account.uipath.com/oauth/token";

        #endregion Private and internal fields

        #region Constructors, initiation, etc.

        /// <summary>
        /// Create instance by Access data.
        /// </summary>
        /// <param name="tenantLogicalName"></param>
        /// <param name="clientId"></param>
        /// <param name="userKey"></param>
        public UiPathCloudAPI(string tenantLogicalName, string clientId, string userKey)
            : this()
        {
            TenantLogicalName = tenantLogicalName;
            ClientId = clientId;
            UserKey = userKey;
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        public UiPathCloudAPI()
        {
            SentDataStore = new Queue<string>();
        }

        ~UiPathCloudAPI()
        {
        }

        /// <summary>
        /// Initiation. authorize + get main data.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Initiation(string tenantLogicalName, string clientId = null, string userKey = null)
        {
            Authorization(tenantLogicalName, clientId, userKey);
            GetMainData();
        }

        /// <summary>
        /// UiPath authorize.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Authorization(string tenantLogicalName = null, string clientId = null, string userKey = null)
        {
            if (!string.IsNullOrEmpty(tenantLogicalName))
            {
                TenantLogicalName = tenantLogicalName;
            }
            if (!string.IsNullOrEmpty(clientId))
            {
                ClientId = clientId;
            }
            if (!string.IsNullOrEmpty(userKey))
            {
                UserKey = userKey;
            }
            if (string.IsNullOrWhiteSpace(TenantLogicalName) || string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(UserKey))
            {
                throw new ArgumentException("Tenant Logical Name or Client Id or User Key is empty.");
            }

            var authParametr = new AuthParameters
            {
                ClientId = ClientId,
                RefreshToken = UserKey
            };
            string output = JsonConvert.SerializeObject(authParametr);
            var sentData = Encoding.UTF8.GetBytes(output);
            Token = JsonConvert.DeserializeObject<AuthToken>(SendRequestPost(urlUipathAuth, sentData, true));
            Authorized = true;
        }

        /// <summary>
        /// Get main data for next operations.
        /// </summary>
        public void GetMainData()
        {
            TargetUser = GetAccountsForUser();
            if (!TargetUser.Accounts.Any())
            {
                throw new Exception("Accounts for target user is empty.");
            }
            TargetAccount = TargetUser.Accounts.First();
            if (string.IsNullOrWhiteSpace(TenantLogicalName))
            {
                throw new Exception("LogicalName is null, empty or white space filled.");
            }
            GetAllServiceInstances();
            if (!ServiceInstances.Any())
            {
                throw new Exception("ServiceInstances is empty.");
            }
            TargetServiceInstance = ServiceInstances.First();
        }

        #endregion Constructors, initiation, etc.

        #region Accounts

        /// <summary>
        /// Get accounts for target user.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetAccountsForTargetUser()
        {
            List<Account> accounts = new List<Account>();

            if (Authorized)
            {
                accounts.AddRange(TargetUser.Accounts);
            }
            else
            {
                LastErrorMessage = "No authorized";
            }

            return accounts;
        }

        /// <summary>
        /// Set target account.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool SetTargetAccount(Account account)
        {
            bool result = false;

            if (Authorized)
            {
                if (TargetUser.Accounts.Contains(account))
                {
                    TargetAccount = account;
                    result = true;
                }
                else
                {
                    LastErrorMessage = "The specified account does not belong to the target user.";
                }
            }
            else
            {
                LastErrorMessage = "No authorized";
            }

            return result;
        }

        #endregion Accounts

        #region Jobs

        /// <summary>
        /// Start new job by robot and proccess release.
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public List<Job> StartJob(Robot robot, Process process)
        {
            return StartJob(robot.Id, process.Key);
        }

        /// <summary>
        /// Start new job by robot and proccess release.
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="process"></param>
        /// <param name="inputArguments">Input Arguments</param>
        /// <returns></returns>
        public List<Job> StartJob(Robot robot, Process process, Dictionary<string, object> inputArguments)
        {
            return StartJob(robot.Id, process.Key, inputArguments);
        }

        /// <summary>
        /// Start new job by robot name and proccess name.
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public List<Job> StartJob(string robotName, string processName)
        {
            return StartJob(robotName, processName, new Dictionary<string, object>());
        }

        /// <summary>
        /// Start new job by robot name and proccess name.
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="processName"></param>
        /// <param name="inputArguments">Input Arguments</param>
        /// <returns></returns>
        public List<Job> StartJob(string robotName, string processName, Dictionary<string, object> inputArguments)
        {
            Robot robot = GetRobots().Where(x => x.Name == robotName).FirstOrDefault();
            if (robot == null)
            {
                LastErrorMessage = "The specified robot was not found.";
                return new List<Job>();
            }
            Process process = GetProcesses().Where(x => x.Name == processName).FirstOrDefault();
            if (process == null)
            {
                LastErrorMessage = "The specified proccess was not found.";
                return new List<Job>();
            }
            return StartJob(robot.Id, process.Key, inputArguments);
        }

        /// <summary>
        /// Start new job by robot id and proccess release key.
        /// </summary>
        /// <param name="robotId">Robot ID</param>
        /// <param name="releaseKey">Proccess release key</param>
        /// <returns></returns>
        public List<Job> StartJob(int robotId, string releaseKey)
        {
            return StartJob(robotId, releaseKey, new Dictionary<string, object>());
        }

        /// <summary>
        /// Start new job by robot id and proccess release key.
        /// </summary>
        /// <param name="robotId">Robot ID</param>
        /// <param name="releaseKey">Proccess release key</param>
        /// <param name="inputArguments">Input Arguments</param>
        /// <returns></returns>
        public List<Job> StartJob(int robotId, string releaseKey, Dictionary<string, object> inputArguments)
        {
            List<Job> jobs = new List<Job>();
            if (string.IsNullOrEmpty(releaseKey))
            {
                LastErrorMessage = "Proccess release key is empty.";
            }
            else
            {
                string output = "";
                if (inputArguments.Any())
                {
                    int incount = inputArguments.Count;
                    string correctedInputArguments = "{";
                    foreach (var item in inputArguments)
                    {
                        incount--;
                        correctedInputArguments += '"' + item.Key + "\":\"" + item.Value.ToString() + '"';
                        if (incount > 0)
                            correctedInputArguments += ',';
                    }
                    correctedInputArguments += '}';
                    var startInfo = new StartInfoContainer<StartJobsInfoWithArguments>
                    {
                        StartJobsInfo = new StartJobsInfoWithArguments
                        {
                            ReleaseKey = releaseKey,
                            Strategy = "Specific",
                            RobotIds = new int[] { robotId },
                            InputArgumentsAsString = correctedInputArguments
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
                SentDataStore.Enqueue(output);
                byte[] sentData = Encoding.UTF8.GetBytes(output);
                string returnStr = null;
                try
                {
                    returnStr = SendRequestPostForOdata("Jobs/UiPath.Server.Configuration.OData.StartJobs", sentData);
                    jobs.AddRange(JsonConvert.DeserializeObject<Info<Job>>(returnStr).Items);
                }
                catch (Exception ex)
                {
                    LastErrorMessage += "\nException message:\n" + ex.Message;
                    if (!string.IsNullOrEmpty(returnStr))
                    {
                        LastErrorMessage += "\nReturn string:\n" + returnStr;
                    }
                }
            }
            return jobs;
        }

        /// <summary>
        /// Get job list
        /// </summary>
        /// <returns></returns>
        public List<Job> GetJobs()
        {
            string response = SendRequestGetForOdata("Jobs");
            return JsonConvert.DeserializeObject<Info<Job>>(response).Items;
        }

        /// <summary>
        /// Get job list
        /// </summary>
        /// <param name="clauses">Clauses</param>
        /// <returns></returns>
        public List<Job> GetJobs(IClause clauses)
        {
            string response = SendRequestGetForOdata("Jobs", clauses);
            return JsonConvert.DeserializeObject<Info<Job>>(response).Items;
        }

        /// <summary>
        /// Get job list
        /// </summary>
        /// <param name="clauses">Clauses</param>
        /// <returns></returns>
        public List<Job> GetJobs(string conditions)
        {
            Filter filter = new Filter(conditions);
            string response = SendRequestGetForOdata("Jobs", filter);
            return JsonConvert.DeserializeObject<Info<Job>>(response).Items;
        }

        #endregion Jobs

        #region Assets

        /// <summary>
        /// Get Assets.
        /// </summary>
        /// <returns></returns>
        public List<ConcreteAsset> GetConcreteAssets()
        {
            return GetAssets().Select(x => x.Concrete()).ToList();
        }

        /// <summary>
        /// Get Assets.
        /// </summary>
        /// <returns></returns>
        public List<Asset> GetAssets()
        {
            string response = SendRequestGetForOdata("Assets");
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Items;
        }

        /// <summary>
        /// Get robot Asset.
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Asset GetRobotAsset(Robot robot, string assetName)
        {
            return GetRobotAssetByRobotId(robot.Id, assetName);
        }

        /// <summary>
        /// Get robot Asset.
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Asset GetRobotAsset(string robotName, string assetName)
        {
            Robot robot = GetRobots().Where(x => x.Name == robotName).FirstOrDefault();
            if (robot == null)
            {
                LastErrorMessage = "The specified robot was not found.";
                return null;
            }
            return GetRobotAssetByRobotId(robot.Id, assetName);
        }

        /// <summary>
        /// Get robot Asset.
        /// </summary>
        /// <param name="robotId"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Asset GetRobotAssetByRobotId(int robotId, string assetName)
        {
            string response = SendRequestGetForOdata(string.Format("Assets/UiPath.Server.Configuration.OData.GetRobotAssetByRobotId(robotId={0},assetName='{1}')", robotId, assetName));
            return JsonConvert.DeserializeObject<Asset>(response);
        }

        #endregion Assets

        #region Robots

        /// <summary>
        /// Get robot by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Robot GetRobot(int id)
        {
            string response = SendRequestGetForOdata(string.Format("Robots({0})", id));
            return JsonConvert.DeserializeObject<Robot>(response);
        }

        /// <summary>
        /// Get robot list using sessions (extended robots information)
        /// </summary>
        /// <returns></returns>
        public List<Robot> GetRobots()
        {
            return GetExtendedRobotsInfo().Select(x => x.Robot).ToList();
        }

        /// <summary>
        /// Get robot list using sessions (extended robots information)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Robot> GetRobots(Filter filter)
        {
            return GetExtendedRobotsInfo(filter).Select(x => x.Robot).ToList();
        }

        /// <summary>
        /// Get robot list using sessions (extended robots information)
        /// </summary>
        /// <param name="clauses"></param>
        /// <returns></returns>
        public List<Robot> GetRobots(ODataClauses clauses)
        {
            return GetExtendedRobotsInfo(clauses).Select(x => x.Robot).ToList();
        }

        /// <summary>
        /// Get robot list
        /// </summary>
        /// <returns></returns>
        public List<Robot> GetRobots2()
        {
            string response = SendRequestGetForOdata("Robots");
            return JsonConvert.DeserializeObject<Info<Robot>>(response).Items;
        }

        /// <summary>
        /// Get extended robots information
        /// </summary>
        /// <returns></returns>
        public List<RobotInfo> GetExtendedRobotsInfo()
        {
            ODataClauses clauses = new ODataClauses(select: "Robot", expand: "Robot");
            string response = SendRequestGetForOdata("Sessions", clauses);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        public List<RobotInfo> GetExtendedRobotsInfo(Filter filter)
        {
            ODataClauses clauses = new ODataClauses(filter: filter, select: "Robot", expand: "Robot");
            string response = SendRequestGetForOdata("Sessions", clauses);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        /// <summary>
        /// Get extended robots information with clauses
        /// </summary>
        /// <param name="top">Top</param>
        /// <param name="filter">Filter</param>
        /// <param name="select">Select</param>
        /// <param name="expand">Expand</param>
        /// <param name="orderby">OrderBy</param>
        /// <param name="skip">Skip</param>
        /// <returns></returns>
        public List<RobotInfo> GetExtendedRobotsInfo(int top = -1, Filter filter = null, OrderBy orderBy = null, string skip = null)
        {
            string response = SendRequestGetForOdata("Sessions", top, filter, "Robot", "Robot", orderBy, skip);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        /// <summary>
        /// Get extended robots information with clauses
        /// </summary>
        /// <param name="clauses">Session clauses</param>
        /// <returns></returns>
        public List<RobotInfo> GetExtendedRobotsInfo(ODataClauses clauses)
        {
            ODataClauses sessionClauses = clauses;
            if (string.IsNullOrEmpty(sessionClauses.Select))
            {
                sessionClauses.Select = "Robot";
            }
            else if (sessionClauses.Select != "Robot")
            {
                throw new ArgumentException("Select != \"Robot\"");
            }
            if (string.IsNullOrEmpty(sessionClauses.Expand))
            {
                sessionClauses.Expand = "Robot";
            }
            else if (sessionClauses.Expand != "Robot")
            {
                throw new ArgumentException("Expand != \"Robot\"");
            }
            string response = SendRequestGetForOdata("Sessions", sessionClauses);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        #endregion Robots

        #region Processes

        /// <summary>
        /// Get a list of all processes
        /// </summary>
        /// <returns></returns>
        public List<Process> GetProcesses()
        {
            string response = SendRequestGetForOdata("Releases");
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        /// <summary>
        /// Get a list of all processes by condition string
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public List<Process> GetProcesses(string conditions)
        {
            Filter filter = new Filter(conditions);
            return GetProcesses(filter);
        }

        /// <summary>
        /// Get a list of all processes by condition
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="objectValue"></param>
        /// <param name="comparisonOperator"></param>
        /// <returns></returns>
        public List<Process> GetProcesses(string objectName, object objectValue, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            Filter filter = new Filter(objectName, objectValue, comparisonOperator);
            return GetProcesses(filter);
        }

        /// <summary>
        /// Get a list of all processes by condition
        /// </summary>
        /// <param name="objectBaseName"></param>
        /// <param name="objectPropertyName"></param>
        /// <param name="objectValue"></param>
        /// <param name="comparisonOperator"></param>
        /// <returns></returns>
        public List<Process> GetProcesses(string objectBaseName, string objectPropertyName, object objectValue, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            Filter filter = new Filter(objectBaseName, objectPropertyName, objectValue, comparisonOperator);
            return GetProcesses(filter);
        }

        /// <summary>
        /// Get a list of all processes by OData clauses
        /// </summary>
        /// <param name="clauses"></param>
        /// <returns></returns>
        public List<Process> GetProcesses(IClause clauses)
        {
            string response = SendRequestGetForOdata("Releases", clauses);
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        /// <summary>
        /// Get a list of all processes by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public List<Process> GetProcessesByName(string name)
        {
            Filter filter = new Filter("Name", name);
            return GetProcesses(filter);
        }

        #endregion Processes

        #region Libraries

        public List<Library> GetLibraries()
        {
            string response = SendRequestGetForOdata("Libraries");
            return JsonConvert.DeserializeObject<Info<Library>>(response).Items;
        }

        public List<Library> GetLibraries(string conditions)
        {
            Filter filter = new Filter(conditions);
            return GetLibraries(filter);
        }

        public List<Library> GetLibraries(string objectName, object objectValue, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            Filter filter = new Filter(objectName, objectValue, comparisonOperator);
            return GetLibraries(filter);
        }
        
        public List<Library> GetLibraries(string objectBaseName, string objectPropertyName, object objectValue, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            Filter filter = new Filter(objectBaseName, objectPropertyName, objectValue, comparisonOperator);
            return GetLibraries(filter);
        }

        public List<Library> GetLibraries(IClause clauses)
        {
            string response = SendRequestGetForOdata("Libraries", clauses);
            return JsonConvert.DeserializeObject<Info<Library>>(response).Items;
        }

        #endregion Libraries

        #region Process Schedules

        public List<Schedule> GetProcessSchedules()
        {
            string response = SendRequestGetForOdata("ProcessSchedules");
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Items;
        }

        public Schedule GetProcessSchedule(int id)
        {
            string response = SendRequestGetForOdata(string.Format("ProcessSchedules({0})", id));
            return JsonConvert.DeserializeObject<Schedule>(response);
        }

        #endregion Process Schedules

        #region Private methods
        private string SendRequestGetForOdata(string operationPart, int top = -1, Filter filter = null, string select = null, string expand = null, OrderBy orderBy = null, string skip = null)
        {
            ODataClauses clauses = new ODataClauses(top, filter, select, expand, orderBy, skip);
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, clauses.GetODataString()));
        }

        private string SendRequestGetForOdata(string operationPart, IClause clauses)
        {
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, clauses.GetODataString()));
        }

        private string SendRequestGetForOdata(string operationPart)
        {
            if (!Authorized)
            {
                throw new Exception("Not authorized.");
            }
            return SendRequestGet(
                string.Format(
                    "https://platform.uipath.com/{0}/{1}/odata/{2}",
                    TargetAccount.LogicalName,
                    TargetServiceInstance.LogicalName,
                    operationPart
                    )
                    , true
                );
        }

        private string SendRequestPostForOdata(string operationPart, byte[] sentData)
        {
            if (!Authorized)
            {
                throw new Exception("Not authorized.");
            }
            return SendRequestPost(
                string.Format(
                    "https://platform.uipath.com/{0}/{1}/odata/{2}",
                    TargetAccount.LogicalName,
                    TargetServiceInstance.LogicalName,
                    operationPart
                    )
                    , sentData
                    , true
                );
        }

        private void GetAllServiceInstances()
        {
            ServiceInstances = JsonConvert.DeserializeObject<List<ServiceInstance>>(
                SendRequestGet(
                    string.Format(
                        "https://platform.uipath.com/cloudrpa/api/account/{0}/getAllServiceInstances", 
                        TargetAccount.LogicalName
                    )
                )
            );
        }

        private AccountsForUser GetAccountsForUser()
        {
            return JsonConvert.DeserializeObject<AccountsForUser>(SendRequestGet("https://platform.uipath.com/cloudrpa/api/getAccountsForUser"));
        }

        private string SendRequestGet(string url, bool access = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Url is empty.");
            }

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "GET";
            req.Timeout = RequestTimeout;
            req.Headers.Add("Authorization", Token.TokenType + " " + (access ? Token.AccessToken : Token.IdToken));
            if (access)
            {
                req.Headers.Add("X-UIPATH-TenantName", TargetServiceInstance.LogicalName);
            }

            return SendRequest(req);
        }

        private string SendRequestPost(string url, byte[] sentData, bool access = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Url is empty.");
            }

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "POST";
            req.Timeout = RequestTimeout;
            req.ContentType = "application/json";
            req.Accept = "application/json";
            if (access)
            {
                if (Authorized)
                {
                    req.Headers.Add("Authorization", Token.TokenType + " " + Token.AccessToken);
                    req.Headers.Add("X-UIPATH-TenantName", TargetServiceInstance.LogicalName);
                }
                else
                {
                    req.Headers.Add("X-UIPATH-TenantName", TenantLogicalName);
                }
            }

            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);

            return SendRequest(req);
        }

        private string SendRequest(HttpWebRequest httpWebRequest)
        {
            try
            {
                var res = httpWebRequest.GetResponse() as HttpWebResponse;
                var resStream = res.GetResponseStream();
                return new StreamReader(resStream, Encoding.UTF8).ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            LastErrorMessage = reader.ReadToEnd();
                            try
                            {
                                LastIssueResponse = JsonConvert.DeserializeObject<Response>(LastErrorMessage);
                            }
                            catch { }
                        }
                    }
                }
                throw ex;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                throw ex;
            }
        }

        #endregion Private methods
    }
}
