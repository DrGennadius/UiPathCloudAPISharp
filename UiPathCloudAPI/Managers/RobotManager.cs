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

        public IEnumerable<Robot> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Robot> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            if (UseSession)
            {
                return GetInfoCollection().Select(r => r.Robot);
            }
            else
            {
                return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
            }
        }

        public IEnumerable<Robot> GetCollection(IQueryParameters queryParameters)
        {
            if (UseSession)
            {
                return GetInfoCollection(queryParameters).Select(r => r.Robot);
            }
            else
            {
                string response = _requestExecutor.SendRequestGetForOdata("Robots", queryParameters);
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

        public IEnumerable<RobotInfo> GetInfoCollection(string conditions)
        {
            return _sessionManager.GetRobotCollection(conditions);
        }

        public IEnumerable<RobotInfo> GetInfoCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            if (_sessionManager == null)
            {
                _sessionManager = new SessionManager(_requestExecutor);
            }
            return _sessionManager.GetRobotCollection(top, filter, select, expand, orderby, skip);
        }

        public IEnumerable<RobotInfo> GetInfoCollection(IQueryParameters queryParameters)
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
            return _sessionManager.GetRobotCollection(queryParameters);
        }

        public Robot GetInstance(int id)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Robots({0})", id));
            return JsonConvert.DeserializeObject<Robot>(response);
        }

        public Robot GetInstance(Robot instance)
        {
            return GetInstance(instance.Id);
        }

        public int Count()
        {
            if (UseSession)
            {
                return _sessionManager.RobotCount();
            }
            else
            {
                QueryParameters queryParameters = new QueryParameters(top: 0);
                string response = _requestExecutor.SendRequestGetForOdata("Robots", queryParameters);
                return JsonConvert.DeserializeObject<Info<Robot>>(response).Count;
            }
        }
    }
}
