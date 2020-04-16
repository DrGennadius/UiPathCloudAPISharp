using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UiPathCloudAPISharp.Common;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Managers
{
    public class RobotManager : IManager, IGetRequest<Robot>
    {
        public bool UseSession { get; set; }

        private RequestExecutor _requestExecutor;

        private SessionManager _sessionManager;

        internal RobotManager(RequestExecutor requestExecutor, SessionManager sessionManager, bool useSession)
            : this(requestExecutor, useSession)
        {
            _sessionManager = sessionManager;
        }

        internal RobotManager(RequestExecutor requestExecutor, bool useSession)
        {
            _requestExecutor = requestExecutor;
            UseSession = useSession;
        }

        public QueryStore QueryStore
        {
            get;
            private set;
        }

        public IEnumerable<Robot> GetCollection()
        {
            if (UseSession)
            {
                if (_sessionManager == null)
                {
                    _sessionManager = new SessionManager(_requestExecutor);
                }
                return GetInfoCollection().Select(r => r.Robot);
            }
            else
            {
                string response = _requestExecutor.SendRequestGetForOdata("Robots");
                return JsonConvert.DeserializeObject<Info<Robot>>(response).Items;
            }
        }

        public IEnumerable<Robot> GetCollection(Folder folder)
        {
            if (UseSession)
            {
                if (_sessionManager == null)
                {
                    _sessionManager = new SessionManager(_requestExecutor);
                }
                return GetInfoCollection(folder).Select(r => r.Robot);
            }
            else
            {
                string response = _requestExecutor.SendRequestGetForOdata("Robots", folder);
                return JsonConvert.DeserializeObject<Info<Robot>>(response).Items;
            }
        }

        public IEnumerable<Robot> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<Robot> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            if (UseSession)
            {
                return GetInfoCollection(folder).Select(r => r.Robot);
            }
            else
            {
                return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
            }
        }

        public IEnumerable<Robot> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            if (UseSession)
            {
                return GetInfoCollection(queryParameters, folder).Select(r => r.Robot);
            }
            else
            {
                string response = _requestExecutor.SendRequestGetForOdata("Robots", queryParameters, folder);
                return JsonConvert.DeserializeObject<Info<Robot>>(response).Items;
            }
        }

        public IEnumerable<RobotInfo> GetInfoCollection()
        {
            if (_sessionManager == null)
            {
                _sessionManager = new SessionManager(_requestExecutor);
            }
            return _sessionManager.GetRobotCollection();
        }

        public IEnumerable<RobotInfo> GetInfoCollection(Folder folder)
        {
            if (_sessionManager == null)
            {
                _sessionManager = new SessionManager(_requestExecutor);
            }
            return _sessionManager.GetRobotCollection(folder);
        }

        public IEnumerable<RobotInfo> GetInfoCollection(string conditions, Folder folder = null)
        {
            return _sessionManager.GetRobotCollection(conditions, folder);
        }

        public IEnumerable<RobotInfo> GetInfoCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            if (_sessionManager == null)
            {
                _sessionManager = new SessionManager(_requestExecutor);
            }
            return _sessionManager.GetRobotCollection(top, filter, select, expand, orderby, skip, folder);
        }

        public IEnumerable<RobotInfo> GetInfoCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            if (_sessionManager == null)
            {
                _sessionManager = new SessionManager(_requestExecutor);
            }
            if (queryParameters is QueryParameters)
            {
                var queryParametersInstance = queryParameters as QueryParameters;
                if (queryParametersInstance.OrderBy != null)
                {
                    queryParametersInstance.OrderBy.Value = "Robot/" + queryParametersInstance.OrderBy.Value;
                }
                if (queryParametersInstance.Filter != null)
                {
                    if (queryParametersInstance.Filter is Filter)
                    {
                        Filter filter = queryParametersInstance.Filter as Filter;
                        foreach (var item in filter.ConditionLine)
                        {
                            if (item is Condition)
                            {
                                Condition condition = item as Condition;
                                condition.BaseName = "Robot";
                            }
                        }
                    }
                }
                // TODO: Other query parameters correction.
            }
            return _sessionManager.GetRobotCollection(queryParameters, folder);
        }

        public Robot GetInstance(int id, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Robots({0})", id), folder);
            return JsonConvert.DeserializeObject<Robot>(response);
        }

        public Robot GetInstance(Robot instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public int Count(Folder folder = null)
        {
            if (UseSession)
            {
                return _sessionManager.RobotCount(folder);
            }
            else
            {
                QueryParameters queryParameters = new QueryParameters(top: 0);
                string response = _requestExecutor.SendRequestGetForOdata("Robots", queryParameters, folder);
                return JsonConvert.DeserializeObject<Info<Robot>>(response).Count;
            }
        }

        public IEnumerable<Robot> GetRobotsForProcess(Process process, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Robots/UiPath.Server.Configuration.OData.GetRobotsForProcess(processId='{0}')", process.ProcessKey), folder);
            return JsonConvert.DeserializeObject<Info<Robot>>(response).Items;
        }

        /// <summary>
        /// Get logs. Max 1000 for getting collection in one time.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RobotLog> GetLogs(Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("RobotLogs", folder);
            return JsonConvert.DeserializeObject<Info<RobotLog>>(response).Items;
        }

        public IEnumerable<RobotLog> GetLogs(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("RobotLogs", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<RobotLog>>(response).Items;
        }

        public IEnumerable<RobotLog> GetLogs(string robotName, Folder folder = null)
        {
            Filter filter = new Filter();
            return GetLogs(robotName, filter, folder);
        }

        public IEnumerable<RobotLog> GetLogs(string robotName, IQueryParameters queryParameters, Folder folder = null)
        {
            if (queryParameters is QueryParameters)
            {
                var query = queryParameters as QueryParameters;
                if (query.Filter == null)
                {
                    query.Filter = new Filter("RobotName", robotName);
                }
                else if (query.Filter is IFilter)
                {
                    query.Filter = CorrectFilterForLogs(query.Filter as IFilter, robotName);
                }
            }
            else if (queryParameters is IFilter)
            {
                queryParameters = CorrectFilterForLogs(queryParameters as IFilter, robotName);
            }
            string response = _requestExecutor.SendRequestGetForOdata("RobotLogs", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<RobotLog>>(response).Items;
        }

        public IEnumerable<RobotLog> GetLogs(Robot instance, Folder folder = null)
        {
            return GetLogs(instance.Name, folder);
        }

        public IEnumerable<RobotLog> GetLogs(Robot instance, IQueryParameters queryParameters, Folder folder = null)
        {
            return GetLogs(instance.Name, queryParameters, folder);
        }

        public int LogCount(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("RobotLogs", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<RobotLog>>(response).Count;
        }

        public int LogCount(string robotName, Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0, filter: new Filter("RobotName", robotName));
            string response = _requestExecutor.SendRequestGetForOdata("RobotLogs", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<RobotLog>>(response).Count;
        }

        public int LogCount(Robot instance, Folder folder = null)
        {
            return LogCount(instance.Name, folder);
        }

        private IFilter CorrectFilterForLogs(IFilter filter, string robotName)
        {
            if (filter is Filter)
            {
                var mfilter = filter as Filter;
                var condition = mfilter.ConditionLine.Where(x => x is Condition)
                    .Select(e => e as Condition)
                    .Where(c => c.Name == robotName)
                    .FirstOrDefault();
                if (condition == null)
                {
                    mfilter.AddCondition("RobotName", robotName);
                }
                else
                {
                    condition.Value = robotName;
                }
            }
            return new Filter("RobotName", robotName);
        }
    }
}
