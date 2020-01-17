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
        public string UserKey { get { return _requestManager.UserKey; } }

        /// <summary>
        /// Client Id for connect to UiPath Orchestrator via Cloud API.
        /// </summary>
        public string ClientId { get { return _requestManager.ClientId; } }

        /// <summary>
        /// Tenant Logical Name for connect to UiPath Orchestrator via Cloud API.
        /// </summary>
        public string TenantLogicalName { get { return _requestManager.TenantLogicalName; } }

        /// <summary>
        /// Last error message that occurred
        /// </summary>
        public string LastErrorMessage { get { return _requestManager.LastErrorMessage; } }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for the <see cref="HttpWebRequest.GetResponse"/> 
        /// and <see cref="HttpWebRequest.GetRequestStream"/> methods.
        /// <para >The default value is 30,000 milliseconds (30 seconds).</para>
        /// </summary>
        public int RequestTimeout
        {
            get { return _requestManager.RequestTimeout; }
            set { _requestManager.RequestTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for wait the operation.
        /// </summary>
        public int WaitTimeout
        {
            get { return _requestManager.WaitTimeout; }
            set { _requestManager.WaitTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for wait the big operation.
        /// </summary>
        public int BigWaitTimeout
        {
            get { return _requestManager.BigWaitTimeout; }
            set { _requestManager.BigWaitTimeout = value; }
        }

        /// <summary>
        /// Passed an authentication?
        /// </summary>
        public bool IsAuthorized { get { return _requestManager.IsAuthorized; } }

        /// <summary>
        /// Is expired timeout?
        /// </summary>
        public bool IsExpired { get { return _requestManager.IsExpired; } }

        /// <summary>
        /// Last issue response (deserialized).
        /// </summary>
        public Response LastIssueResponse { get { return _requestManager.LastIssueResponse; } }

        /// <summary>
        /// Current target account.
        /// </summary>
        public Account TargetAccount { get { return _requestManager.TargetAccount; } }

        /// <summary>
        /// Current target service instance.
        /// </summary>
        public ServiceInstance TargetServiceInstance { get { return _requestManager.TargetServiceInstance; } }

        /// <summary>
        /// The behavior mode affects the logic of initialization, authorization, and call requests.
        /// </summary>
        public BehaviorMode BehaviorMode { get { return _requestManager.BehaviorMode; } }

        /// <summary>
        /// Robot Manager
        /// </summary>
        public RobotManager RobotManager { get; private set; }

        /// <summary>
        /// Session Manager
        /// </summary>
        public SessionManager SessionManager { get; private set; }

        /// <summary>
        /// Configuration Manager
        /// </summary>
        public UiPathConfigurationManager ConfigurationManager { get; private set; }

        /// <summary>
        /// Process Manager
        /// </summary>
        public ProcessManager ProcessManager { get; private set; }

        /// <summary>
        /// Library Manager
        /// </summary>
        public LibraryManager LibraryManager { get; private set; }

        /// <summary>
        /// Asset Manager
        /// </summary>
        public AssetManager AssetManager { get; private set; }

        /// <summary>
        /// Schedule Manager
        /// </summary>
        public ScheduleManager ScheduleManager { get; private set; }

        /// <summary>
        /// Job Manager
        /// </summary>
        public JobManager JobManager { get; private set; }

        /// <summary>
        /// Transaction Manager
        /// </summary>
        public TransactionManager TransactionManager { get; private set; }

        /// <summary>
        /// Environment Manager
        /// </summary>
        public EnvironmentManager EnvironmentManager { get; private set; }

        /// <summary>
        /// Machine Manager
        /// </summary>
        public MachineManager MachineManager { get; private set; }

        #endregion Public fields

        #region Private and internal properties

        private RequestManager _requestManager;

        private bool _useInitiation = true;

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
            : this(tenantLogicalName, clientId, userKey, null, behaviorMode)
        {
        }

        /// <summary>
        /// Create instance by Access data.
        /// </summary>
        /// <param name="tenantLogicalName"></param>
        /// <param name="clientId"></param>
        /// <param name="userKey"></param>
        /// <param name="accountLogicalName"></param>
        /// <param name="behaviorMode"></param>
        public UiPathCloudAPI(string tenantLogicalName, string clientId, string userKey, string accountLogicalName, BehaviorMode behaviorMode = BehaviorMode.Default)
            : this()
        {
            _useInitiation = false;
            Initialization(tenantLogicalName, clientId, userKey, accountLogicalName, behaviorMode);
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        public UiPathCloudAPI()
        {
            _requestManager = new RequestManager();
        }

        ~UiPathCloudAPI()
        {
        }

        /// <summary>
        /// (Re)Initialization. Initialize managers and if call out of constructor when authorize + get main data.
        /// </summary>
        /// <param name="tenantLogicalName"></param>
        /// <param name="clientId"></param>
        /// <param name="userKey"></param>
        /// <param name="behaviorMode"></param>
        public void Initialization(string tenantLogicalName, string clientId, string userKey, BehaviorMode behaviorMode = BehaviorMode.Default)
        {
            Initialization(tenantLogicalName, clientId, userKey, null, behaviorMode);
        }

        /// <summary>
        /// (Re)Initialization. Initialize managers and if call out of constructor when authorize + get main data.
        /// </summary>
        /// <param name="tenantLogicalName"></param>
        /// <param name="clientId"></param>
        /// <param name="userKey"></param>
        /// <param name="accountLogicalName"></param>
        /// <param name="behaviorMode"></param>
        public void Initialization(string tenantLogicalName, string clientId, string userKey, string accountLogicalName, BehaviorMode behaviorMode = BehaviorMode.Default)
        {
            var _storedRequestTimeout = _requestManager.RequestTimeout;
            var _storedWaitTimeout = _requestManager.WaitTimeout;
            var _storedBigWaitTimeout = _requestManager.BigWaitTimeout;
            _requestManager = new RequestManager(tenantLogicalName, clientId, userKey, accountLogicalName, behaviorMode)
            {
                RequestTimeout = _storedRequestTimeout,
                WaitTimeout = _storedWaitTimeout,
                BigWaitTimeout = _storedBigWaitTimeout
            };
            if (_useInitiation && behaviorMode != BehaviorMode.Default)
            {
                _requestManager.Initiation();
            }
            else
            {
                _useInitiation = true;
            }
            SessionManager = new SessionManager(_requestManager);
            ConfigurationManager = new UiPathConfigurationManager(_requestManager);
            RobotManager = new RobotManager(_requestManager, SessionManager, true);
            ProcessManager = new ProcessManager(_requestManager);
            LibraryManager = new LibraryManager(_requestManager);
            AssetManager = new AssetManager(_requestManager);
            ScheduleManager = new ScheduleManager(_requestManager);
            JobManager = new JobManager(_requestManager, RobotManager, ProcessManager);
            TransactionManager = new TransactionManager(_requestManager);
            EnvironmentManager = new EnvironmentManager(_requestManager);
            MachineManager = new MachineManager(_requestManager);
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
            return _requestManager.GetAccountsForTargetUser();
        }

        /// <summary>
        /// Set target account.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool SetTargetAccount(Account account)
        {
            return _requestManager.SetTargetAccount(account);
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
