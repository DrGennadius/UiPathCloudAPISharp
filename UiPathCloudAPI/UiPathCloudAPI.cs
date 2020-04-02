using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UiPathCloudAPISharp.Common;
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
        /// Last error message that occurred
        /// </summary>
        public string LastErrorMessage
        {
            get
            {
                if (_requestExecutor == null)
                {
                    return _lastErrorMessage;
                }
                else
                {
                    return _requestExecutor.LastErrorMessage;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for the <see cref="HttpWebRequest.GetResponse"/> 
        /// and <see cref="HttpWebRequest.GetRequestStream"/> methods.
        /// <para >The default value is 30,000 milliseconds (30 seconds).</para>
        /// </summary>
        public int RequestTimeout
        {
            get { return _requestExecutor.RequestTimeout; }
            set { _requestExecutor.RequestTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for wait the operation.
        /// </summary>
        public int WaitTimeout
        {
            get { return _requestExecutor.WaitTimeout; }
            set { _requestExecutor.WaitTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the time-out value in milliseconds for wait the big operation.
        /// </summary>
        public int BigWaitTimeout
        {
            get { return _requestExecutor.BigWaitTimeout; }
            set { _requestExecutor.BigWaitTimeout = value; }
        }

        /// <summary>
        /// Passed an authentication?
        /// </summary>
        public bool IsAuthorized { get { return _requestExecutor.IsAuthorized; } }

        /// <summary>
        /// Is expired timeout?
        /// </summary>
        public bool IsExpired { get { return _requestExecutor.IsExpired; } }

        /// <summary>
        /// Last issue response (deserialized).
        /// </summary>
        public Response LastIssueResponse { get { return _requestExecutor.LastIssueResponse; } }

        /// <summary>
        /// Current target account.
        /// </summary>
        public Account TargetAccount { get { return _requestExecutor.TargetAccount; } }

        /// <summary>
        /// Current target service instance.
        /// </summary>
        public ServiceInstance TargetServiceInstance { get { return _requestExecutor.TargetServiceInstance; } }

        /// <summary>
        /// The behavior mode affects the logic of initialization, authorization, and call requests.
        /// </summary>
        public BehaviorMode BehaviorMode { get { return _requestExecutor.BehaviorMode; } }

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

        public bool IsInitialized { get; private set; }

        #endregion Public fields

        #region Private and internal properties

        private RequestExecutor _requestExecutor;

        private bool _useInitiation = true;
        private string _lastErrorMessage;

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
        /// Create instance by configuration.
        /// </summary>
        public UiPathCloudAPI(CloudConfiguration configuration)
        {
            _lastErrorMessage = "";
            IsInitialized = false;
            _requestExecutor = new RequestExecutor(configuration);
        }

        /// <summary>
        /// Create instance.
        /// </summary>
        public UiPathCloudAPI()
        {
            _lastErrorMessage = "";
            IsInitialized = false;
            _requestExecutor = new RequestExecutor(new CloudConfiguration());
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
            CloudConfiguration configuration = new CloudConfiguration
            {
                TenantLogicalName = tenantLogicalName,
                ClientId = clientId,
                UserKey = userKey,
                AccountLogicalName = accountLogicalName,
                BehaviorMode = behaviorMode
            };
            Initialization(configuration);
        }

        public void Initialization(CloudConfiguration configuration)
        {
            bool requestExecutorWasNull = true;
            int storedRequestTimeout = -1;
            int storedWaitTimeout = -1;
            int storedBigWaitTimeout = -1;
            if (_requestExecutor != null)
            {
                requestExecutorWasNull = false;
                storedRequestTimeout = _requestExecutor.RequestTimeout;
                storedWaitTimeout = _requestExecutor.WaitTimeout;
                storedBigWaitTimeout = _requestExecutor.BigWaitTimeout;
            }
            _requestExecutor = new RequestExecutor(configuration);
            if (!requestExecutorWasNull)
            {
                _requestExecutor.RequestTimeout = storedRequestTimeout;
                _requestExecutor.WaitTimeout = storedWaitTimeout;
                _requestExecutor.BigWaitTimeout = storedBigWaitTimeout;
            }
            if (_useInitiation && configuration.BehaviorMode != BehaviorMode.Default)
            {
                try
                {
                    _requestExecutor.Initiation();
                }
                catch (Exception ex)
                {
                    _lastErrorMessage = _requestExecutor.LastErrorMessage;
                    End();
                    throw ex;
                }
            }
            else
            {
                _useInitiation = true;
            }
            SessionManager = new SessionManager(_requestExecutor);
            ConfigurationManager = new UiPathConfigurationManager(_requestExecutor);
            RobotManager = new RobotManager(_requestExecutor, SessionManager, true);
            ProcessManager = new ProcessManager(_requestExecutor);
            LibraryManager = new LibraryManager(_requestExecutor);
            AssetManager = new AssetManager(_requestExecutor);
            ScheduleManager = new ScheduleManager(_requestExecutor);
            JobManager = new JobManager(_requestExecutor, RobotManager, ProcessManager);
            TransactionManager = new TransactionManager(_requestExecutor);
            EnvironmentManager = new EnvironmentManager(_requestExecutor);
            MachineManager = new MachineManager(_requestExecutor);
            IsInitialized = true;
        }

        /// <summary>
        /// Initiation. authorize + get main data.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Initiation(string tenantLogicalName = null, string clientId = null, string userKey = null)
        {
            _requestExecutor.Initiation();
        }

        /// <summary>
        /// UiPath authorize.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void Authorization(string tenantLogicalName = null, string clientId = null, string userKey = null)
        {
            _requestExecutor.Authorization();
        }

        /// <summary>
        /// Get main data for next operations.
        /// </summary>
        public void GetMainData()
        {
            _requestExecutor.GetMainData();
        }

        /// <summary>
        /// Finish work.
        /// </summary>
        public void End()
        {
            IsInitialized = false;
            ConfigurationManager = null;
            JobManager = null;
            RobotManager = null;
            SessionManager = null;
            ProcessManager = null;
            LibraryManager = null;
            AssetManager = null;
            ScheduleManager = null;
            TransactionManager = null;
            EnvironmentManager = null;
            MachineManager = null;
            _requestExecutor = null;
        }

#if !NET40
        public void EnableUsingExtendSecurityProtocolTypes()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public void DisableUsingExtendSecurityProtocolTypes()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
        }
#endif

        #endregion Constructors, initiation, etc.

        #region Accounts

        /// <summary>
        /// Get accounts for target user.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetAccountsForTargetUser()
        {
            return _requestExecutor.GetAccountsForTargetUser();
        }

        /// <summary>
        /// Set target account.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool SetTargetAccount(Account account)
        {
            return _requestExecutor.SetTargetAccount(account);
        }

#endregion Accounts
 
#region Private methods

        private AccountsForUser GetAccountsForUser()
        {
            return JsonConvert.DeserializeObject<AccountsForUser>(_requestExecutor.SendRequestGet("https://platform.uipath.com/cloudrpa/api/getAccountsForUser"));
        }

#endregion Private methods
    }
}
