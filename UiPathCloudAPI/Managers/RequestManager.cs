using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Managers
{
    internal class RequestManager
    {
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
        /// Gets or sets the time-out value in milliseconds for wait the big operation.
        /// </summary>
        public int WaitTimeout { get; set; } = 300000;

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for wait the big operation.
        /// </summary>
        public int BigWaitTimeout { get; set; } = 1800000;

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
        public BehaviorMode BehaviorMode { get; private set; }

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

        private AccountsForUser TargetUser { get; set; }

        private bool _isAuthorized = false;

        private DateTime _expirationTime;

        private readonly string urlUipathAuth = "https://account.uipath.com/oauth/token";

        /// <summary>
        /// Reserve of time in seconds for expiration time.
        /// </summary>
        private readonly int _leeway = 30;

        public RequestManager(string tenantLogicalName, string clientId, string userKey, BehaviorMode behaviorMode = BehaviorMode.Default)
        {
            TenantLogicalName = tenantLogicalName;
            ClientId = clientId;
            UserKey = userKey;
            BehaviorMode = behaviorMode;
        }

        /// <summary>
        /// Initiation. authorize + get main data.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Initiation(string tenantLogicalName = null, string clientId = null, string userKey = null)
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
            IsAuthorized = false;
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

        public string SendRequestGetForOdata(string operationPart, int top = -1, Filter filter = null, string select = null, string expand = null, OrderBy orderBy = null, string skip = null)
        {
            QueryParameters clauses = new QueryParameters(top, filter, select, expand, orderBy, skip);
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, clauses.GetQueryString()));
        }

        public string SendRequestGetForOdata(string operationPart, IQueryParameters queryParameters)
        {
            return SendRequestGetForOdata(string.Format("{0}?{1}", operationPart, queryParameters.GetQueryString()));
        }

        private string SendRequestGetForOdata(string operationPart)
        {
            if (!IsAuthorized)
            {
                if (BehaviorMode == BehaviorMode.AutoAuthorization)
                {
                    Authorization(TenantLogicalName, ClientId, UserKey);
                }
                else if (BehaviorMode == BehaviorMode.AutoInitiation)
                {
                    Initiation(TenantLogicalName, ClientId, UserKey);
                }
            }
            if (!IsAuthorized)
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

        public string SendRequestPostForOdata(string operationPart, byte[] sentData)
        {
            if (!IsAuthorized)
            {
                if (BehaviorMode == BehaviorMode.AutoAuthorization)
                {
                    Authorization(TenantLogicalName, ClientId, UserKey);
                }
                else if (BehaviorMode == BehaviorMode.AutoInitiation)
                {
                    Initiation(TenantLogicalName, ClientId, UserKey);
                }
            }
            if (!IsAuthorized)
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
    }

    /// <summary>
    /// The behavior mode affects the logic of initialization, authorization, and call requests.
    /// </summary>
    public enum BehaviorMode
    {
        /// <summary>
        /// No use automatic initiation and authorization.
        /// </summary>
        Default,

        /// <summary>
        /// Automatic initiation when trying to execute a request if not yet authorized or timeout token life.
        /// </summary>
        AutoInitiation,

        /// <summary>
        /// Automatic authorization when trying to execute a request if not yet authorized or timeout token life.
        /// </summary>
        AutoAuthorization
    }
}
