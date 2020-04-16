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

        private RequestExecutor _requestExecutor;

        internal ProcessManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<Process> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Releases");
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        public IEnumerable<Process> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Releases", folder);
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        public IEnumerable<Process> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<Process> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<Process> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Releases", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Process>>(response).Items;
        }

        public Process GetInstance(int id, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Releases({0})", id), folder);
            return JsonConvert.DeserializeObject<Process>(response);
        }

        public Process GetInstance(Process instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Releases", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Process>>(response).Count;
        }

        //public void ChangeInstance(Process instance)
        //{
        //    string output = JsonConvert.SerializeObject(instance);
        //    byte[] sentData = Encoding.UTF8.GetBytes(output);
        //    _requestExecutor.SendRequestPatchForOdata(string.Format("Releases({0})", instance.Id), sentData);
        //}
    }
}
