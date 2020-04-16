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

        public IEnumerable<Machine> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Machines", folder);
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Items;
        }

        public IEnumerable<Machine> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<Machine> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<Machine> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Machines", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Items;
        }

        public Machine GetInstance(int id, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Machines({0})", id), folder);
            return JsonConvert.DeserializeObject<Machine>(response);
        }

        public Machine GetInstance(Machine instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Machines", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Count;
        }
    }
}
