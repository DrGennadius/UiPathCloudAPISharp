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

        public IEnumerable<Schedule> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("ProcessSchedules", folder);
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Items;
        }

        public IEnumerable<Schedule> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<Schedule> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<Schedule> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("ProcessSchedules", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Items;
        }

        public Schedule GetInstance(int id, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("ProcessSchedules({0})", id), folder);
            return JsonConvert.DeserializeObject<Schedule>(response);
        }

        public Schedule GetInstance(Schedule instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("ProcessSchedules", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Schedule>>(response).Count;
        }
    }
}
