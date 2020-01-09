using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UiPathCloudAPISharp.Managers;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

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
        public string UserKey => _requestManager.UserKey;

        /// <summary>
        /// Client Id for connect to UiPath Orchestrator via Cloud API.
        /// </summary>
        public string ClientId => _requestManager.ClientId;

        /// <summary>
        /// Tenant Logical Name for connect to UiPath Orchestrator via Cloud API.
        /// </summary>
        public string TenantLogicalName => _requestManager.TenantLogicalName;

        /// <summary>
        /// Last error message that occurred
        /// </summary>
        public string LastErrorMessage => _requestManager.LastErrorMessage;

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
        /// Passed an authentication?
        /// </summary>
        public bool IsAuthorized => _requestManager.IsAuthorized;

        /// <summary>
        /// Is expired timeout?
        /// </summary>
        public bool IsExpired => _requestManager.IsExpired;

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

        /// <summary>
        /// The behavior mode affects the logic of initialization, authorization, and call requests.
        /// </summary>
        public BehaviorMode BehaviorMode => _requestManager.BehaviorMode;
        
        public RobotManager RobotManager { get; private set; }
        public SessionManager SessionManager { get; private set; }
        public ProcessManager ProcessManager { get; private set; }
        public LibraryManager LibraryManager { get; private set; }
        public AssetManager AssetManager { get; private set; }
        public ScheduleManager ScheduleManager { get; private set; }
        public JobManager JobManager { get; private set; }

        #endregion Public fields

        #region Private and internal properties

        internal AuthToken Token { get; set; }

        private RequestManager _requestManager;

        private List<ServiceInstance> ServiceInstances { get; set; }

        private AccountsForUser TargetUser { get; set; }

        private bool _isAuthorized = false;

        private DateTime _expirationTime;

        private readonly string urlUipathAuth = "https://account.uipath.com/oauth/token";

        /// <summary>
        /// Reserve of time in seconds for expiration time.
        /// </summary>
        private readonly int _leeway = 30;

        #endregion Private and internal fields

        #region Constructors, initiation, etc.

        /// <summary>
        /// Create instance by Access data.
        /// </summary>
        /// <param name="tenantLogicalName"></param>
        /// <param name="clientId"></param>
        /// <param name="userKey"></param>
        /// <param name="behaviorMode"></param>
        public UiPathCloudAPI(string tenantLogicalName, string clientId, string userKey, BehaviorMode behaviorMode = BehaviorMode.Default)
            : this()
        {
            _requestManager = new RequestManager(tenantLogicalName, clientId, userKey, behaviorMode);
            SessionManager = new SessionManager(_requestManager);
            RobotManager = new RobotManager(_requestManager, SessionManager, true);
            ProcessManager = new ProcessManager(_requestManager);
            LibraryManager = new LibraryManager(_requestManager);
            AssetManager = new AssetManager(_requestManager);
            ScheduleManager = new ScheduleManager(_requestManager);
            JobManager = new JobManager(_requestManager, RobotManager, ProcessManager);
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
        public void Initiation(string tenantLogicalName = null, string clientId = null, string userKey = null)
        {
            _requestManager.Initiation();
        }

        /// <summary>
        /// UiPath authorize.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Authorization(string tenantLogicalName = null, string clientId = null, string userKey = null)
        {
            _requestManager.Authorization();
        }

        /// <summary>
        /// Get main data for next operations.
        /// </summary>
        public void GetMainData()
        {
            _requestManager.GetMainData();
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

        #endregion Accounts
 
        #region Private methods

        private AccountsForUser GetAccountsForUser()
        {
            return JsonConvert.DeserializeObject<AccountsForUser>(_requestManager.SendRequestGet("https://platform.uipath.com/cloudrpa/api/getAccountsForUser"));
        }
        
        #endregion Private methods
    }
}
