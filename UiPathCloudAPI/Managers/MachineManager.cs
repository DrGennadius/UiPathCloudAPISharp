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
    public class MachineManager : IManager, IGetRequest<Machine>
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestExecutor _requestExecutor;

        internal MachineManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<Machine> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Machines");
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Items;
        }

        public IEnumerable<Machine> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Machine> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Machine> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Machines", queryParameters);
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Items;
        }

        public Machine GetInstance(int id)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Machines({0})", id));
            return JsonConvert.DeserializeObject<Machine>(response);
        }

        public Machine GetInstance(Machine instance)
        {
            return GetInstance(instance.Id);
        }

        public int Count()
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Machines", queryParameters);
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Count;
        }
    }
}
