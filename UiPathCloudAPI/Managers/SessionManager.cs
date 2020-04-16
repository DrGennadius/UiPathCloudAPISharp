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

        public IEnumerable<Session> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", folder);
            return JsonConvert.DeserializeObject<Info<Session>>(response).Items;
        }

        public IEnumerable<Session> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<Session> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<Session> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Session>>(response).Items;
        }

        public IEnumerable<RobotInfo> GetRobotCollection()
        {
            QueryParameters queryParameters = new QueryParameters(select: "Robot", expand: "Robot");
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", queryParameters);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        public IEnumerable<RobotInfo> GetRobotCollection(Folder folder)
        {
            QueryParameters queryParameters = new QueryParameters(select: "Robot", expand: "Robot");
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
        }

        public IEnumerable<RobotInfo> GetRobotCollection(string conditions, Folder folder = null)
        {
            return GetRobotCollection(new Filter(conditions), folder);
        }

        public IEnumerable<RobotInfo> GetRobotCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetRobotCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<RobotInfo> GetRobotCollection(IQueryParameters queryParameters, Folder folder = null)
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
                return GetRobotCollection(folder);
            }
            else
            {
                commonQueryParameters.Select = "Robot";
                commonQueryParameters.Expand = "Robot";
                string response = _requestExecutor.SendRequestGetForOdata("Sessions", commonQueryParameters, folder);
                return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Items;
            }
        }

        public Session GetInstance(int id, Folder folder = null)
        {
            return GetCollection(folder).Where(s => s.Id == id).FirstOrDefault();
        }

        public Session GetInstance(Session instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Session>>(response).Count;
        }

        public int RobotCount(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters
            {
                Top = 0,
                Expand = "Robot"
            };
            string response = _requestExecutor.SendRequestGetForOdata("Sessions", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<RobotInfo>>(response).Count;
        }
    }
}
