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
    public class LibraryManager : IManager
    {
        public QueryStore QueryStore => throw new NotImplementedException();

        private RequestManager _requestManager;

        internal LibraryManager(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        public IEnumerable<Library> GetCollection()
        {
            string response = _requestManager.SendRequestGetForOdata("Libraries");
            return JsonConvert.DeserializeObject<Info<Library>>(response).Items;
        }

        public IEnumerable<Library> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Library> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Library> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestManager.SendRequestGetForOdata("Libraries", queryParameters);
            return JsonConvert.DeserializeObject<Info<Library>>(response).Items;
        }
    }
}
