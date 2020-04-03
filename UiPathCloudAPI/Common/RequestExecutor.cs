using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Common
{
    internal class RequestExecutor
    {
        public RequestExecutor(string tenantLogicalName, string clientId, string userKey, BehaviorMode behaviorMode = BehaviorMode.Default)
            : this(tenantLogicalName, clientId, userKey, null, behaviorMode)
        {
        }

        public RequestExecutor(string tenantLogicalName, string clientId, string userKey, string accountLogicalName, BehaviorMode behaviorMode = BehaviorMode.Default)
            : this(
                  new CloudConfiguration
                  {
                      TenantLogicalName = tenantLogicalName,
                      ClientId = clientId,
                      UserKey = userKey,
                      AccountLogicalName = accountLogicalName,
                      BehaviorMode = behaviorMode
                  }
                  )
        {
        }

        public RequestExecutor(CloudConfiguration configuration)
        {
            Configuration = configuration;
            if (!string.IsNullOrEmpty(configuration.AccountLogicalName))
            {
                _requiredAccountLogicalName = configuration.AccountLogicalName;
            }
            if (string.IsNullOrEmpty(configuration.BaseURL))
            {
                configuration.BaseURL = urlUiPathDefault;
            }
            RequestTimeout = 30000;
            WaitTimeout = 300000;
            BigWaitTimeout = 1800000;
        }

        /// <summary>
        /// Configuration.
        /// </summary>
        public CloudConfiguration Configuration { get; private set; }
        
        /// <summary>
        /// Last error message that occurred
        /// </summary>
        public string LastErrorMessage { get; private set; }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for the <see cref="HttpWebRequest.GetResponse"/> 
        /// and <see cref="HttpWebRequest.GetRequestStream"/> methods.
        /// <para >The default value is 30,000 milliseconds (30 seconds).</para>
        /// </summary>
        public int RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for wait the big operation.
        /// </summary>
        public int WaitTimeout { get; set; }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for wait the big operation.
        /// </summary>
        public int BigWaitTimeout { get; set; }

        /// <summary>
        /// Target User.
        /// </summary>
        public AccountsForUser TargetUser { get; set; }

        /// <summary>
        /// Current target account.
        /// </summary>
        public Account TargetAccount { get; private set; }

        /// <summary>
        /// Current target service instance.
        /// </summary>
        public ServiceInstance TargetServiceInstance { get; private set; }

        /// <summary>
        /// The behavior mode affects the logic of initialization, authorization, and call requests.
        /// </summary>
        public BehaviorMode BehaviorMode
        {
            get
            {
                return Configuration.BehaviorMode;
            }
            private set
            {
                Configuration.BehaviorMode = value;
            }
        }
        
        /// <summary>
        /// Last issue response (deserialized).
        /// </summary>
        public Response LastIssueResponse { get; private set; }

        /// <summary>
        /// Passed an authentication?
        /// </summary>
        public bool IsAuthorized
        {
            get
            {
                if (IsExpired)
                {
                    _isAuthorized = false;
                }
                return _isAuthorized;
            }
            private set
            {
                _isAuthorized = value;
            }
        }

        /// <summary>
        /// Is expired timeout?
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return DateTime.Now >= _expirationTime;
            }
        }

        internal AuthToken Token { get; set; }

        private List<ServiceInstance> ServiceInstances { get; set; }

        private bool _isAuthorized = false;

        private DateTime _expirationTime;

        private readonly string urlUiPathDefault = "https://platform.uipath.com";

        private string _requiredAccountLogicalName = null;

        /// <summary>
        /// Reserve of time in seconds for expiration time.
        /// </summary>
        private readonly int _leeway = 30;

        /// <summary>
        /// Initiation. authorize + get main data.
        /// </summary>
        /// <param name="tenantLogicalName"></param>
        /// <param name="clientId"></param>
        /// <param name="userKey"></param>
        /// <param name="accountLogicalName"></param>
        public void Initiation(string tenantLogicalName = null, string clientId = null, string userKey = null, string accountLogicalName = null)
        {
            Authorization(tenantLogicalName, clientId, userKey, accountLogicalName);
            GetMainData();
        }

        /// <summary>
        /// UiPath authorization.
        /// </summary>
        /// <param name="tenantLogicalName"></param>
        /// <param name="clientId"></param>
        /// <param name="userKey"></param>
        /// <param name="accountLogicalName"></param>
        public void Authorization(string tenantLogicalName = null, string clientId = null, string userKey = null, string accountLogicalName = null)
        {
            IsAuthorized = false;
            if (!string.IsNullOrEmpty(tenantLogicalName))
            {
                Configuration.TenantLogicalName = tenantLogicalName;
            }
            if (!string.IsNullOrEmpty(clientId))
            {
                Configuration.ClientId = clientId;
            }
            if (!string.IsNullOrEmpty(userKey))
            {
                Configuration.UserKey = userKey;
            }
            if (string.IsNullOrWhiteSpace(Configuration.TenantLogicalName) || string.IsNullOrWhiteSpace(Configuration.ClientId) || string.IsNullOrWhiteSpace(Configuration.UserKey))
            {
                throw new ArgumentException("Tenant Logical Name or Client Id or User Key is empty.");
            }
            _requiredAccountLogicalName = accountLogicalName;

            var authParametr = new AuthParameters
            {
                ClientId = Configuration.ClientId,
                RefreshToken = Configuration.UserKey
            };
            string output = JsonConvert.SerializeObject(authParametr);
            var sentData = Encoding.UTF8.GetBytes(output);
            string result = SendRequestPost(Configuration.BaseURL + "https://platform.uipath.com/oauth/token", sentData, true);
            Token = JsonConvert.DeserializeObject<AuthToken>(result);
            _expirationTime = DateTime.Now.AddSeconds(Token.ExpiresIn - _leeway);
            IsAuthorized = true;
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
            if (string.IsNullOrEmpty(_requiredAccountLogicalName))
            {
                TargetAccount = TargetUser.Accounts.First();
            }
            else
            {
                TargetAccount = TargetUser.Accounts.Where(a => a.LogicalName == _requiredAccountLogicalName).FirstOrDefault();
                if (TargetAccount == null)
                {
                    throw new Exception("Required Account is not found.");
                }
            }
            if (string.IsNullOrWhiteSpace(Configuration.TenantLogicalName))
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

        /// <summary>
        /// Get accounts for target user.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetAccountsForTargetUser()
        {
            List<Account> accounts = new List<Account>();

            if (IsAuthorized)
            {
                accounts.AddRange(TargetUser.Accounts);
            }
            else
            {
                throw new Exception("No authorized");
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

            if (IsAuthorized)
            {
                if (TargetUser.Accounts.Contains(account))
                {
                    TargetAccount = account;
                    result = true;
                }
                else
                {
                    throw new Exception("The specified account does not belong to the target user.");
                }
            }
            else
            {
                throw new Exception("No authorized");
            }

            return result;
        }

        public string SendRequestGetForOdata(string operationPart, int top = -1, Filter filter = null, string select = null, string expand = null, OrderBy orderBy = null, int skip = -1)
        {
            QueryParameters clauses = new QueryParameters(top, filter, select, expand, orderBy, skip);
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, clauses.GetQueryString()));
        }

        public string SendRequestGetForOdata(string operationPart, IQueryParameters queryParameters)
        {
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, queryParameters.GetQueryString()));
        }

        public string SendRequestGetForOdata(string operationPart)
        {
            if (!IsAuthorized)
            {
                if (BehaviorMode == BehaviorMode.AutoAuthorization)
                {
                    Authorization(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
                else if (BehaviorMode == BehaviorMode.AutoInitiation)
                {
                    Initiation(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
            }
            if (!IsAuthorized)
            {
                throw new Exception("Not authorized.");
            }
            return SendRequestGet(
                string.Format(
                    "{0}/{1}/{2}/odata/{3}",
                    Configuration.BaseURL,
                    TargetAccount.LogicalName,
                    TargetServiceInstance.LogicalName,
                    operationPart
                    )
                    , true
                );
        }

        public string SendRequestPostForOdata(string operationPart, byte[] sentData)
        {
            if (!IsAuthorized)
            {
                if (BehaviorMode == BehaviorMode.AutoAuthorization)
                {
                    Authorization(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
                else if (BehaviorMode == BehaviorMode.AutoInitiation)
                {
                    Initiation(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
            }
            if (!IsAuthorized)
            {
                throw new Exception("Not authorized.");
            }
            return SendRequestPost(
                string.Format(
                    "{0}/{1}/{2}/odata/{3}",
                    Configuration.BaseURL,
                    TargetAccount.LogicalName,
                    TargetServiceInstance.LogicalName,
                    operationPart
                    )
                    , sentData
                    , true
                );
        }

        public void SendRequestPutForOdata(string operationPart, byte[] sentData)
        {
            if (!IsAuthorized)
            {
                if (BehaviorMode == BehaviorMode.AutoAuthorization)
                {
                    Authorization(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
                else if (BehaviorMode == BehaviorMode.AutoInitiation)
                {
                    Initiation(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
            }
            if (!IsAuthorized)
            {
                throw new Exception("Not authorized.");
            }
            SendRequestPut(
                string.Format(
                    "{0}/{1}/{2}/odata/{3}",
                    Configuration.BaseURL,
                    TargetAccount.LogicalName,
                    TargetServiceInstance.LogicalName,
                    operationPart
                    )
                    , sentData
                    , true
                );
        }

        public void SendRequestDeleteForOdata(string operationPart)
        {
            if (!IsAuthorized)
            {
                if (BehaviorMode == BehaviorMode.AutoAuthorization)
                {
                    Authorization(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
                else if (BehaviorMode == BehaviorMode.AutoInitiation)
                {
                    Initiation(Configuration.TenantLogicalName, Configuration.ClientId, Configuration.UserKey);
                }
            }
            if (!IsAuthorized)
            {
                throw new Exception("Not authorized.");
            }
            SendRequestDelete(
                string.Format(
                    "{0}/{1}/{2}/odata/{3}",
                    Configuration.BaseURL,
                    TargetAccount.LogicalName,
                    TargetServiceInstance.LogicalName,
                    operationPart
                    )
                    , true
                );
        }

        private void GetAllServiceInstances()
        {
            ServiceInstances = JsonConvert.DeserializeObject<List<ServiceInstance>>(
                SendRequestGet(
                    string.Format(
                        "{0}/cloudrpa/api/account/{1}/getAllServiceInstances",
                        Configuration.BaseURL,
                        TargetAccount.LogicalName
                    )
                )
            );
        }

        private AccountsForUser GetAccountsForUser()
        {
            return JsonConvert.DeserializeObject<AccountsForUser>(SendRequestGet(Configuration.BaseURL + "/cloudrpa/api/getAccountsForUser"));
        }

        public string SendRequestGet(string url, bool access = false)
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
                if (IsAuthorized)
                {
                    req.Headers.Add("Authorization", Token.TokenType + " " + Token.AccessToken);
                    req.Headers.Add("X-UIPATH-TenantName", TargetServiceInstance.LogicalName);
                }
                else
                {
                    req.Headers.Add("X-UIPATH-TenantName", Configuration.TenantLogicalName);
                }
            }

            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);

            return SendRequest(req);
        }

        private void SendRequestPut(string url, byte[] sentData, bool access = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Url is empty.");
            }

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "PUT";
            req.Timeout = RequestTimeout;
            req.ContentType = "application/json";
            req.Accept = "application/json";
            if (access)
            {
                if (IsAuthorized)
                {
                    req.Headers.Add("Authorization", Token.TokenType + " " + Token.AccessToken);
                    req.Headers.Add("X-UIPATH-TenantName", TargetServiceInstance.LogicalName);
                }
                else
                {
                    req.Headers.Add("X-UIPATH-TenantName", Configuration.TenantLogicalName);
                }
            }

            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);

            string responseString = SendRequest(req);
        }

        private void SendRequestDelete(string url, bool access = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Url is empty.");
            }

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "DELETE";
            req.Timeout = RequestTimeout;
            req.ContentType = "application/json";
            req.Accept = "application/json";
            if (access)
            {
                if (IsAuthorized)
                {
                    req.Headers.Add("Authorization", Token.TokenType + " " + Token.AccessToken);
                    req.Headers.Add("X-UIPATH-TenantName", TargetServiceInstance.LogicalName);
                }
                else
                {
                    req.Headers.Add("X-UIPATH-TenantName", Configuration.TenantLogicalName);
                }
            }

            string responseString = SendRequest(req);
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
    }
}
