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
    public class ScheduleManager : IManager, IGetRequest<Schedule>
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestExecutor _requestExecutor;

        internal ScheduleManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<Schedule> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("ProcessSchedules");
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Items;
        }

        public IEnumerable<Schedule> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Schedule> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Schedule> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestExecutor.SendRequestGetForOdata("ProcessSchedules", queryParameters);
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Items;
        }

        public Schedule GetInstance(int id)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("ProcessSchedules({0})", id));
            return JsonConvert.DeserializeObject<Schedule>(response);
        }

        public Schedule GetInstance(Schedule instance)
        {
            return GetInstance(instance.Id);
        }

        public int Count()
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("ProcessSchedules", queryParameters);
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Count;
        }
    }
}
