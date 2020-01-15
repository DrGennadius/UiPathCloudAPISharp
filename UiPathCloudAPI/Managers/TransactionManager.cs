using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Common;
using UiPathCloudAPISharp.Managers;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Managers
{
    public class TransactionManager : IManager, IGetRequest<QueueItem>
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestManager _requestManager;

        internal TransactionManager(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        /// <summary>
        /// Get Queue items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QueueItem> GetCollection()
        {
            string response = _requestManager.SendRequestGetForOdata("QueueItems");
            return JsonConvert.DeserializeObject<Info<QueueItem>>(response).Items;
        }

        public IEnumerable<QueueItem> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<QueueItem> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<QueueItem> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestManager.SendRequestGetForOdata("QueueItems", queryParameters);
            return JsonConvert.DeserializeObject<Info<QueueItem>>(response).Items;
        }

        public QueueItem GetInstance(int id)
        {
            string response = _requestManager.SendRequestGetForOdata(string.Format("QueueItems({0})", id));
            return JsonConvert.DeserializeObject<QueueItem>(response);
        }

        public QueueItem GetInstance(QueueItem instance)
        {
            return GetInstance(instance.Id);
        }
    }
}
