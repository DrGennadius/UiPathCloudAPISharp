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

        public IEnumerable<UiPathEnvironment> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<UiPathEnvironment> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<UiPathEnvironment> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Environments", queryParameters);
            return JsonConvert.DeserializeObject<Info<UiPathEnvironment>>(response).Items;
        }

        public UiPathEnvironment GetInstance(int id)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Environments({0})", id));
            return JsonConvert.DeserializeObject<UiPathEnvironment>(response);
        }

        public UiPathEnvironment GetInstance(UiPathEnvironment instance)
        {
            return GetInstance(instance.Id);
        }

        public int Count()
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Environments", queryParameters);
            return JsonConvert.DeserializeObject<Info<UiPathEnvironment>>(response).Count;
        }
    }
}
