using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Common;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Managers
{
    public class SessionManager : IManager, IGetRequest<Session>
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestExecutor _requestExecutor;

        internal SessionManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<Session> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Sessions");
            return JsonConvert.DeserializeObject<Info<Session>>(response).Items;
        }

        public IEnumerable<Session> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Session> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Session> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", queryParameters);
            return JsonConvert.DeserializeObject<Info<Session>>(response).Items;
        }

        public IEnumerable<RobotInfo> GetRobotCollection()
        {
            QueryParameters queryParameters = new QueryParameters(select: "Robot", expand: "Robot");
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", queryParameters);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        public IEnumerable<RobotInfo> GetRobotCollection(string conditions)
        {
            return GetRobotCollection(new Filter(conditions));
        }

        public IEnumerable<RobotInfo> GetRobotCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetRobotCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<RobotInfo> GetRobotCollection(IQueryParameters queryParameters)
        {
            QueryParameters commonQueryParameters = null;
            if (queryParameters is IFilter)
            {
                commonQueryParameters = new QueryParameters(filter: queryParameters as IFilter);
            }
            else if (queryParameters is QueryParameters)
            {
                commonQueryParameters = queryParameters as QueryParameters;
            }
            if (commonQueryParameters == null)
            {
                return GetRobotCollection();
            }
            else
            {
                commonQueryParameters.Select = "Robot";
                commonQueryParameters.Expand = "Robot";
                string response = _requestExecutor.SendRequestGetForOdata("Sessions", commonQueryParameters);
                return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
            }
        }

        public Session GetInstance(int id)
        {
            return GetCollection().Where(s => s.Id == id).FirstOrDefault();
        }

        public Session GetInstance(Session instance)
        {
            return GetInstance(instance.Id);
        }
    }
}
