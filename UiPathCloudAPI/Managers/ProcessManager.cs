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
    public class ProcessManager : IManager, IGetRequest<Process>
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestManager _requestManager;

        internal ProcessManager(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        public IEnumerable<Process> GetCollection()
        {
            string response = _requestManager.SendRequestGetForOdata("Releases");
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        public IEnumerable<Process> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Process> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Process> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestManager.SendRequestGetForOdata("Releases", queryParameters);
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        public Process GetInstance(int id)
        {
            string response = _requestManager.SendRequestGetForOdata(string.Format("Releases({0})", id));
            return JsonConvert.DeserializeObject<Process>(response);
        }

        public Process GetInstance(Process instance)
        {
            return GetInstance(instance.Id);
        }

        //public void ChangeInstance(Process instance)
        //{
        //    string output = JsonConvert.SerializeObject(instance);
        //    byte[] sentData = Encoding.UTF8.GetBytes(output);
        //    _requestManager.SendRequestPatchForOdata(string.Format("Releases({0})", instance.Id), sentData);
        //}
    }
}
