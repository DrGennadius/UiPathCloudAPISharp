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
    public class EnvironmentManager : IManager, IGetRequest<UiPathEnvironment>
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestExecutor _requestExecutor;

        internal EnvironmentManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<UiPathEnvironment> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Environments");
            return JsonConvert.DeserializeObject<Info<UiPathEnvironment>>(response).Items;
        }

        public IEnumerable<UiPathEnvironment> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Environments", folder);
            return JsonConvert.DeserializeObject<Info<UiPathEnvironment>>(response).Items;
        }

        public IEnumerable<UiPathEnvironment> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<UiPathEnvironment> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<UiPathEnvironment> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Environments", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<UiPathEnvironment>>(response).Items;
        }

        public UiPathEnvironment GetInstance(int id, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Environments({0})", id), folder);
            return JsonConvert.DeserializeObject<UiPathEnvironment>(response);
        }

        public UiPathEnvironment GetInstance(UiPathEnvironment instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Environments", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<UiPathEnvironment>>(response).Count;
        }
    }
}
