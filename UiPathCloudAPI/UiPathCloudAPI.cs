using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace UiPathCloudAPISharp
{
    public class UiPathCloudAPI
    {
        #region Public fields

        /// <summary>
        /// UiPath login/email
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// UiPath Password
        /// </summary>
        public string Password { get; set; }

        public string Code { get; set; }

        public string CodeVerifier { get; set; }

        public string CodeChallenge { get; set; }

        public string ClientId { get; set; } = "5v7PmPJL6FOGu6RB8I1Y4adLBhIwovQN";

        /// <summary>
        /// Last error message that occurred
        /// </summary>
        public string LastErrorMessage { get; private set; }

        public string TargetLogicalName { get; private set; }

        /// <summary>
        /// Passed an authentication?
        /// </summary>
        public bool Authorized { get; private set; } = false;

        /// <summary>
        /// Store for sent JSON data.
        /// </summary>
        public Queue<string> SentDataStore { get; private set; }

        #endregion Public fields

        #region Private and internal fields

        internal AuthToken Token { get; set; }

        private List<ServiceInstance> ServiceInstances { get; set; }

        private ServiceInstance TargetServiceInstance { get; set; }

        private AccountsForUser TargetUser { get; set; }

        private Account TargetAccount { get; set; }

        private IWebDriver WebDriver { get; set; }

        private readonly string patternCode = @"(?<=code=).*(?=&state)";

        private readonly string urlGetCodeBase = "https://account.uipath.com/authorize?response_type=code&nonce=b0f368cbc59c6b99ccc8e9b66a30b4a6&state=47441df4d0f0a89da08d43b6dfdc4be2&code_challenge={0}&code_challenge_method=S256&scope=openid+profile+offline_access+email &audience=https%3A%2F%2Forchestrator.cloud.uipath.com&client_id={1}&redirect_uri=https%3A%2F%2Faccount.uipath.com%2Fmobile";

        private readonly string urlUipathAuth = "https://account.uipath.com/oauth/token";

        #endregion Private and internal fields

        #region Constructors, initiation, etc.

        public UiPathCloudAPI(string login, string password, string clientId, string codeVerifier)
            : this(login, password, clientId)
        {
            CodeVerifier = codeVerifier;
        }

        public UiPathCloudAPI(string login, string password, string clientId)
            : this(login, password)
        {
            ClientId = clientId;
        }

        public UiPathCloudAPI(string login, string password)
            : this()
        {
            Login = login;
            Password = password;
        }

        public UiPathCloudAPI()
        {
            SentDataStore = new Queue<string>();
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("headless");
            chromeOptions.AddArgument("--log-level=3");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            WebDriver = new ChromeDriver(service, chromeOptions);
            ComputeCodes();
        }

        ~UiPathCloudAPI()
        {
            WebDriver.Dispose();
        }

        /// <summary>
        /// Initiation. authorize + get main data.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Initiation(string login = null, string password = null)
        {
            Authorize(login, password);
            GetMainData();
        }

        /// <summary>
        /// UiPath authorize.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Authorize(string login = null, string password = null)
        {
            if (!string.IsNullOrEmpty(login))
            {
                Login = login;
            }
            if (!string.IsNullOrEmpty(password))
            {
                Password = password;
            }
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Login or Password is empty.");
            }

            SeleniumAuthorizeToUiPath();

            var authParametr = new AuthParameters
            {
                code = Code,
                code_verifier = CodeVerifier,
                client_id = ClientId
            };
            string output = JsonConvert.SerializeObject(authParametr);
            var sentData = Encoding.UTF8.GetBytes(output);
            Token = JsonConvert.DeserializeObject<AuthToken>(SendRequestPost(urlUipathAuth, sentData));
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
            TargetLogicalName = TargetUser.Accounts.First().LogicalName;
            if (string.IsNullOrWhiteSpace(TargetLogicalName))
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
                    var startInfo = new StartInfoContainer<StartJobsWithArgumentsInfo>
                    {
                        StartJobsInfo = new StartJobsWithArgumentsInfo
                        {
                            ReleaseKey = releaseKey,
                            Strategy = "Specific",
                            RobotIds = new int[] { robotId },
                            InputArguments = correctedInputArguments
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
        /// Job list
        /// </summary>
        /// <returns></returns>
        public List<Job> GetJobs()
        {
            string response = SendRequestGetForOdata("Jobs");
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
            SessionClauses clauses = new SessionClauses
            {
                Select = "Robot",
                Expand = "Robot"
            };
            string response = SendRequestGetForOdata("Sessions", clauses);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        public List<RobotInfo> GetExtendedRobotsInfo(CommonFilter filter)
        {
            ODataClauses clauses = new ODataClauses
            {
                Filter = filter,
                Select = "Robot",
                Expand = "Robot"
            };
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
        public List<RobotInfo> GetExtendedRobotsInfo(int top = -1, CommonFilter filter = null, string select = null, string expand = null, string orderby = null, string skip = null)
        {
            string response = SendRequestGetForOdata("Sessions", top, filter, select, expand, orderby, skip);
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
        /// Process list
        /// </summary>
        /// <returns></returns>
        public List<Process> GetProcesses()
        {
            string response = SendRequestGetForOdata("Releases");
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        /// <summary>
        /// Process list
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public List<Process> GetProcesses(string name)
        {
            ReleaseFilter filter = new ReleaseFilter
            {
                Name = name
            };
            return GetProcesses(filter);
        }

        /// <summary>
        /// Process list
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns></returns>
        public List<Process> GetProcesses(ReleaseFilter filter)
        {
            string response = SendRequestGetForOdata("Releases", filter);
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        #endregion Processes

        #region Private
        private string SendRequestGetForOdata(string operationPart, int top = -1, CommonFilter filter = null, string select = null, string expand = null, string orderby = null, string skip = null)
        {
            ODataClauses clauses = new ODataClauses(top, filter, select, expand, orderby, skip);
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, clauses.Value));
        }

        private string SendRequestGetForOdata(string operationPart, IClause clauses)
        {
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, clauses.Value));
        }

        private string SendRequestGetForOdata(string operationPart)
        {
            return SendRequestGet(
                string.Format(
                    "https://platform.uipath.com/{0}/{1}/odata/{2}",
                    TargetLogicalName,
                    ServiceInstances.FirstOrDefault().LogicalName,
                    operationPart
                    )
                    , true
                );
        }

        private string SendRequestPostForOdata(string operationPart, byte[] sentData)
        {
            return SendRequestPost(
                string.Format(
                    "https://platform.uipath.com/{0}/{1}/odata/{2}",
                    TargetLogicalName,
                    ServiceInstances.FirstOrDefault().LogicalName,
                    operationPart
                    )
                    , sentData
                    , true
                );
        }

        private void GetAllServiceInstances()
        {
            ServiceInstances = JsonConvert.DeserializeObject<List<ServiceInstance>>(SendRequestGet(string.Format("https://platform.uipath.com/cloudrpa/api/account/{0}/getAllServiceInstances", TargetLogicalName)));
        }

        private AccountsForUser GetAccountsForUser()
        {
            return JsonConvert.DeserializeObject<AccountsForUser>(SendRequestGet("https://platform.uipath.com/cloudrpa/api/getAccountsForUser"));
        }

        private string SendRequestGet(string url, bool access = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Empty url");
            }

            string result = "";

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "GET";
            req.Timeout = 10000;
            req.Headers.Add("Authorization", Token.TokenType + " " + (access ? Token.AccessToken : Token.IdToken));
            if (access)
            {
                req.Headers.Add("X-UIPATH-TenantName", ServiceInstances.FirstOrDefault().LogicalName);
            }

            try
            {
                var res = req.GetResponse() as HttpWebResponse;
                var resStream = res.GetResponseStream();
                result = new StreamReader(resStream, Encoding.UTF8).ReadToEnd();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        LastErrorMessage = reader.ReadToEnd();
                    }
                }
                throw ex;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                throw ex;
            }

            return result;
        }

        private string SendRequestPost(string url, byte[] sentData, bool access = false)
        {
            string result = "";

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "POST";
            req.Timeout = 10000;
            req.ContentType = "application/json";
            req.Accept = "application/json";
            if (access)
            {
                req.Headers.Add("Authorization", Token.TokenType + " " + Token.AccessToken);
                req.Headers.Add("X-UIPATH-TenantName", ServiceInstances.FirstOrDefault().LogicalName);
            }

            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);

            try
            {
                var res = req.GetResponse() as HttpWebResponse;
                var resStream = res.GetResponseStream();
                result = new StreamReader(resStream, Encoding.UTF8).ReadToEnd();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        LastErrorMessage = reader.ReadToEnd();
                    }
                }
                throw ex;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                throw ex;
            }

            return result;
        }

        private void SeleniumAuthorizeToUiPath()
        {
            WebDriver.Url = string.Format(urlGetCodeBase, CodeChallenge, ClientId);
            WebDriver.Manage().Window.Maximize();

            try
            {
                IWebElement emailField = WebDriver.FindElement(By.XPath("//input[@id='text-field-hero-input'][@class='mdc-text-field__input marginNone loginFormEmailText']"));
                IWebElement passwordField = WebDriver.FindElement(By.XPath("//input[@id='text-field-hero-input'][@type='password'][@class='mdc-text-field__input marginNone loginFormPasswordTextField']"));
                IWebElement loginButton = WebDriver.FindElement(By.Id("loginButton"));

                emailField.SendKeys(Login);
                passwordField.SendKeys(Password);
                loginButton.Click();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            int tryCount = 10;
            while (tryCount > 0)
            {
                Thread.Sleep(2000);

                Regex regex = new Regex(patternCode);
                Match match = regex.Match(WebDriver.Url);
                if (match.Success)
                {
                    Code = match.Value;
                    Authorized = true;
                    break;
                }

                tryCount--;
            }

            string url = WebDriver.Url;
            WebDriver.Close();
            WebDriver.Quit();
            if (!Authorized)
            {
                throw new Exception("Error for Selenium Authorize.\nLast ChromeDriver URL: " + url);
            }
        }

        private void ComputeCodes()
        {
            if (string.IsNullOrWhiteSpace(CodeVerifier))
            {
                RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
                byte[] randomBytes = new byte[32];
                rngCsp.GetBytes(randomBytes);
                CodeVerifier = Base64URLEncode(randomBytes);
            }
            CodeChallenge = Base64URLEncode(ComputeSha256Hash(CodeVerifier));
        }

        private byte[] ComputeSha256Hash(string rawData)
        { 
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            }
        }

        private string Base64URLEncode(byte[] rawData)
        {
            Regex regex = new Regex("=");
            return regex.Replace(Convert.ToBase64String(rawData).Replace('+', '-').Replace('/', '_'), "");
        }
        #endregion Private
    }
}
