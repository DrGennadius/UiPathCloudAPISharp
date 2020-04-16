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

        private RequestExecutor _requestExecutor;

        internal TransactionManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        /// <summary>
        /// Get Queue items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QueueItem> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueItems");
            return JsonConvert.DeserializeObject<Info<QueueItem>>(response).Items;
        }

        /// <summary>
        /// Get Queue items from Folder.
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <returns></returns>
        public IEnumerable<QueueItem> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueItems", folder);
            return JsonConvert.DeserializeObject<Info<QueueItem>>(response).Items;
        }

        public IEnumerable<QueueItem> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<QueueItem> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<QueueItem> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueItems", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<QueueItem>>(response).Items;
        }

        public QueueItem GetInstance(int id, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("QueueItems({0})", id), folder);
            return JsonConvert.DeserializeObject<QueueItem>(response);
        }

        public QueueItem GetInstance(QueueItem instance, Folder folder = null)
        {
            return GetInstance(instance.Id, folder);
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("QueueItems", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<QueueItem>>(response).Count;
        }

        public IEnumerable<QueueDefinition> GetQueueDefinitions()
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueDefinitions");
            return JsonConvert.DeserializeObject<Info<QueueDefinition>>(response).Items;
        }

        public IEnumerable<QueueDefinition> GetQueueDefinitions(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueDefinitions", folder);
            return JsonConvert.DeserializeObject<Info<QueueDefinition>>(response).Items;
        }

        public IEnumerable<QueueDefinition> GetQueueDefinitions(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueDefinitions", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<QueueDefinition>>(response).Items;
        }

        public QueueItem AddQueueItem(NewQueueItem queueItem, Folder folder = null)
        {
            string output = JsonConvert.SerializeObject(queueItem);
            byte[] sentData = Encoding.UTF8.GetBytes(output);
            string response = _requestExecutor.SendRequestPostForOdata("Queues/UiPathODataSvc.AddQueueItem", sentData, folder);
            return JsonConvert.DeserializeObject<QueueItem>(response);
        }

        public QueueItem AddQueueItem(NewQueueItemData queueItemData, Folder folder = null)
        {
            NewQueueItem newQueueItem = new NewQueueItem
            {
                ItemData = queueItemData
            };
            return AddQueueItem(newQueueItem, folder);
        }

        public TransactionStatus AddQueueItems(NewMultipleQueueItems newMultipleQueueItems, Folder folder = null)
        {
            string output = JsonConvert.SerializeObject(newMultipleQueueItems);
            byte[] sentData = Encoding.UTF8.GetBytes(output);
            string response = _requestExecutor.SendRequestPostForOdata("Queues/UiPathODataSvc.BulkAddQueueItems", sentData, folder);
            return JsonConvert.DeserializeObject<TransactionStatus>(response);
        }

        #region QueueItemEvents
        public IEnumerable<QueueItemEvent> GetQueueItemEvents()
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueItemEvents");
            return JsonConvert.DeserializeObject<Info<QueueItemEvent>>(response).Items;
        }

        public IEnumerable<QueueItemEvent> GetQueueItemEvents(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueItemEvents", folder);
            return JsonConvert.DeserializeObject<Info<QueueItemEvent>>(response).Items;
        }

        public IEnumerable<QueueItemEvent> GetQueueItemEvents(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("QueueItemEvents", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<QueueItemEvent>>(response).Items;
        }

        public IEnumerable<QueueItemEvent> GetQueueItemEventsHistory(int queueItemId, Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(orderby: new OrderBy("Timestamp"));
            string response = _requestExecutor.SendRequestGetForOdata(
                string.Format("QueueItemEvents/UiPath.Server.Configuration.OData.GetQueueItemEventsHistory(queueItemId={0})", queueItemId),
                queryParameters,
                folder
                );
            return JsonConvert.DeserializeObject<Info<QueueItemEvent>>(response).Items;
        }

        public IEnumerable<QueueItemEvent> GetQueueItemEventsHistory(int queueItemId, IQueryParameters queryParameters, Folder folder = null)
        {
            if (queryParameters is QueryParameters)
            {
                var query = queryParameters as QueryParameters;
                if (query.OrderBy == null)
                {
                    query.OrderBy = new OrderBy("Timestamp");
                }
            }
            else if (queryParameters is IFilter)
            {
                queryParameters = new QueryParameters(filter: queryParameters as IFilter, orderby: new OrderBy("Timestamp"));
            }
            string response = _requestExecutor.SendRequestGetForOdata(
                string.Format("QueueItemEvents/UiPath.Server.Configuration.OData.GetQueueItemEventsHistory(queueItemId={0})", queueItemId),
                queryParameters,
                folder
                );
            return JsonConvert.DeserializeObject<Info<QueueItemEvent>>(response).Items;
        }

        public IEnumerable<QueueItemEvent> GetQueueItemEventsHistory(QueueItem instance, Folder folder = null)
        {
            return GetQueueItemEventsHistory(instance.Id, folder);
        }

        public IEnumerable<QueueItemEvent> GetQueueItemEventsHistory(QueueItem instance, IQueryParameters queryParameters, Folder folder = null)
        {
            return GetQueueItemEventsHistory(instance.Id, queryParameters, folder);
        }

        public QueueItemEvent GetQueueItemEvent(int queueItemEventId, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("QueueItemEvents({0})", queueItemEventId), folder);
            return JsonConvert.DeserializeObject<QueueItemEvent>(response);
        }

        public QueueItemEvent GetQueueItemEvent(QueueItemEvent queueItemEvent, Folder folder = null)
        {
            return GetQueueItemEvent(queueItemEvent.Id, folder);
        }

        public int QueueItemEventCount(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("QueueItemEvents", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<QueueItem>>(response).Count;
        }
        #endregion QueueItemEvents
    }
}
