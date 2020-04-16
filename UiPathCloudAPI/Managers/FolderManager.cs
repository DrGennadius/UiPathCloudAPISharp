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
    /// <summary>
    /// Folder Manager. Get information about Organization Unit. It is need for access to specific folder in requests.
    /// </summary>
    public class FolderManager : IManager
    {
        public QueryStore QueryStore
        {
            get { throw new NotImplementedException(); }
        }

        private RequestExecutor _requestExecutor;

        internal FolderManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<Folder> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Folders");
            return JsonConvert.DeserializeObject<Info<Folder>>(response).Items;
        }
        
        public IEnumerable<Folder> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Folder> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }
        
        public IEnumerable<Folder> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Folders", queryParameters, null);
            return JsonConvert.DeserializeObject<Info<Folder>>(response).Items;
        }

        public Folder GetInstance(int id)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Folders({0})", id), null);
            return JsonConvert.DeserializeObject<Folder>(response);
        }

        public Folder GetInstance(string displayName)
        {
            return GetCollection(new Filter("DisplayName", displayName)).FirstOrDefault();
        }

        public Folder GetInstance(Folder instance)
        {
            return GetInstance(instance.Id);
        }

        public int Count()
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Folders", queryParameters, null);
            return JsonConvert.DeserializeObject<Info<Machine>>(response).Count;
        }
    }
}
