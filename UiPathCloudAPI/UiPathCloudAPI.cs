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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UiPathOrchestrator
{
    public class UiPathCloudAPI
    {
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

        public string LogicalName { get; private set; }

        /// <summary>
        /// Passed an authentication?
        /// </summary>
        public bool Authenticated { get; private set; } = false;

        internal AuthToken Token { get; set; }

        private List<ServiceInstance> ServiceInstances { get; set; }

        private readonly string patternCode = @"(?<=code=).*(?=&state)";

        private readonly string urlGetCodeBase = "https://account.uipath.com/authorize?response_type=code&nonce=b0f368cbc59c6b99ccc8e9b66a30b4a6&state=47441df4d0f0a89da08d43b6dfdc4be2&code_challenge={0}&code_challenge_method=S256&scope=openid+profile+offline_access+email &audience=https%3A%2F%2Forchestrator.cloud.uipath.com&client_id={1}&redirect_uri=https%3A%2F%2Faccount.uipath.com%2Fmobile";

        private readonly string urlUipathAuth = "https://account.uipath.com/oauth/token";

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
            ComputeCodes();
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
        /// UiPath authorize
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

            try
            {
                // TODO: Make without WebBrowser
                RunBrowserThread(string.Format(urlGetCodeBase, CodeChallenge, ClientId));
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                throw ex;
            }

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
        /// Get main data for next operations
        /// </summary>
        public void GetMainData()
        {
            AccountsForUser accountsForUser = GetAccountsForUser();
            LogicalName = accountsForUser.Accounts.FirstOrDefault()?.LogicalName;
            if (string.IsNullOrWhiteSpace(LogicalName))
            {
                throw new Exception("LogicalName is null, empty or white space filled.");
            }
            GetAllServiceInstances();
            if (!ServiceInstances.Any())
            {
                throw new Exception("ServiceInstances is empty.");
            }
        }

        /// <summary>
        /// Start new job by robot id and proccess key.
        /// </summary>
        /// <param name="robotId">Robot ID</param>
        /// <param name="releaseKey"></param>
        /// <returns></returns>
        public List<Job> StartJob(int robotId, string releaseKey = null)
        {
            if (string.IsNullOrEmpty(releaseKey))
            {
                releaseKey = GetProcesses().FirstOrDefault()?.Key;
            }
            var startInfo = new StartInfoContainer<StartJobsInfo>
            {
                StartJobsInfo = new StartJobsInfo
                {
                    ReleaseKey = releaseKey,
                    Strategy = "Specific",
                    RobotIds = new int[] { robotId }
                }
            };
            string output = JsonConvert.SerializeObject(startInfo);
            var sentData = Encoding.UTF8.GetBytes(output);
            var returnStr = SendRequestPostForOdata("Jobs/UiPath.Server.Configuration.OData.StartJobs", sentData);
            return JsonConvert.DeserializeObject<Info<Job>>(returnStr).Items;
        }

        /// <summary>
        /// Robot list
        /// </summary>
        /// <returns></returns>
        public List<Robot> GetRobots()
        {
            string response = SendRequestGetForOdata("Sessions?$select=Robot&$expand=Robot");
            return JsonConvert.DeserializeObject<Info<RobotContainer>>(response).Items.Select(x => x.Robot).ToList();
        }

        /// <summary>
        /// Job list
        /// </summary>
        /// <returns></returns>
        public List<Job> GetJobs()
        {
            return JsonConvert.DeserializeObject<Info<Job>>(SendRequestGetForOdata("Jobs")).Items;
        }

        /// <summary>
        /// Process list
        /// </summary>
        /// <returns></returns>
        public List<Process> GetProcesses()
        {
            var str = SendRequestGetForOdata("Releases");
            return JsonConvert.DeserializeObject<Info<Process>>(str).Items;
        }

        private string SendRequestGetForOdata(string operationPart)
        {
            return SendRequestGet(
                string.Format(
                    "https://platform.uipath.com/{0}/{1}/odata/{2}",
                    LogicalName,
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
                    LogicalName,
                    ServiceInstances.FirstOrDefault().LogicalName,
                    operationPart
                    )
                    , sentData
                    , true
                );
        }

        private void GetAllServiceInstances()
        {
            ServiceInstances = JsonConvert.DeserializeObject<List<ServiceInstance>>(SendRequestGet(string.Format("https://platform.uipath.com/cloudrpa/api/account/{0}/getAllServiceInstances", LogicalName)));
        }

        private AccountsForUser GetAccountsForUser()
        {
            return JsonConvert.DeserializeObject<AccountsForUser>(SendRequestGet("https://platform.uipath.com/cloudrpa/api/getAccountsForUser"));
        }

        private string SendRequestGet(string url, bool access = false)
        {
            string result = "";

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "GET";
            req.Timeout = 10000;
            req.Headers.Add("Authorization", Token.token_type + " " + (access ? Token.access_token : Token.id_token));
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
                req.Headers.Add("Authorization", Token.token_type + " " + Token.access_token);
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

        private void RunBrowserThread(string url)
        {
            var th = new Thread(() =>
            {
                WebBrowser webBrowser = new WebBrowser();
                webBrowser.DocumentCompleted += browser_DocumentCompleted;
                webBrowser.Navigate(url);
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            th.Join();
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var webBrowser = sender as WebBrowser;
            Regex regex = new Regex(patternCode);
            Match match = regex.Match(e.Url.OriginalString);
            if (match.Success)
            {
                Code = match.Value;
                Authenticated = true;
                Application.ExitThread();   // Stops the thread
            }
            if (!Authenticated && webBrowser.Url == e.Url)
            {
                Console.WriteLine("Natigated to {0}", e.Url);
                HtmlElement emailField = null;
                HtmlElement passwordField = null;
                var elements = webBrowser.Document.GetElementsByTagName("input");
                foreach (HtmlElement element in elements)
                {
                    if (element.Id == "text-field-hero-input")
                    {
                        if (element.GetAttribute("type") == "password" && element.GetAttribute("className") == "mdc-text-field__input marginNone loginFormPasswordTextField")
                        {
                            passwordField = element;
                        }
                        else if (element.GetAttribute("className") == "mdc-text-field__input marginNone loginFormEmailText")
                        {
                            emailField = element;
                        }
                        if (emailField != null && passwordField != null)
                        {
                            break;
                        }
                    }
                }
                if (emailField != null && passwordField != null)
                {
                    emailField.SetAttribute("value", Login);
                    passwordField.SetAttribute("value", Password);
                    HtmlElement loginButton = webBrowser.Document.GetElementById("loginButton");
                    loginButton.InvokeMember("login");
                    Authenticated = true;
                }
                else
                {
                    Application.ExitThread();   // Stops the thread
                }
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
    }
}
