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
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestExecutor _requestExecutor;

        internal LibraryManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<Library> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Libraries");
            return JsonConvert.DeserializeObject<Info<Library>>(response).Items;
        }

        public IEnumerable<Library> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Libraries", folder);
            return JsonConvert.DeserializeObject<Info<Library>>(response).Items;
        }

        public IEnumerable<Library> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<Library> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<Library> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Libraries", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Library>>(response).Items;
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Libraries", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Library>>(response).Count;
        }
    }
}
