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

        private RequestManager _requestManager;

        internal ScheduleManager(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        public IEnumerable<Schedule> GetCollection()
        {
            string response = _requestManager.SendRequestGetForOdata("ProcessSchedules");
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Items;
        }

        public IEnumerable<Schedule> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Schedule> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Schedule> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestManager.SendRequestGetForOdata("ProcessSchedules", queryParameters);
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Items;
        }

        public Schedule GetInstance(int id)
        {
            string response = _requestManager.SendRequestGetForOdata(string.Format("ProcessSchedules({0})", id));
            return JsonConvert.DeserializeObject<Schedule>(response);
        }

        public Schedule GetInstance(Schedule instance)
        {
            return GetInstance(instance.Id);
        }
    }
}
