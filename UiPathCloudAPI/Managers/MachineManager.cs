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
        public QueryStore QueryStore => throw new NotImplementedException();

        private RequestManager _requestManager;

        internal MachineManager(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        public IEnumerable<Machine> GetCollection()
        {
            string response = _requestManager.SendRequestGetForOdata("Machines");
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Items;
        }

        public IEnumerable<Machine> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Machine> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Machine> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestManager.SendRequestGetForOdata("Machines", queryParameters);
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Items;
        }

        public Machine GetInstance(int id)
        {
            string response = _requestManager.SendRequestGetForOdata(string.Format("Machines({0})", id));
            return JsonConvert.DeserializeObject<Machine>(response);
        }

        public Machine GetInstance(Machine instance)
        {
            return GetInstance(instance.Id);
        }
    }
}
