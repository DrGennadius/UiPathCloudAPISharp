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
    public class RobotManager : IManager, IGetRequest<Robot>
    {
        public bool UseSession { get; set; }

        private RequestManager _requestManager;

        internal RobotManager(RequestManager requestManager, bool useSession)
        {
            _requestManager = requestManager;
            UseSession = useSession;
        }        

        public QueryStore QueryStore
        {
            get;
            private set;
        }

        public IEnumerable<Robot> GetCollection()
        {
            if (UseSession)
            {
                throw new NotImplementedException();
            }
            else
            {
                string response = _requestManager.SendRequestGetForOdata("Robots");
                return JsonConvert.DeserializeObject<Info<Robot>>(response).Items;
            }
        }

        public IEnumerable<Robot> GetCollection(IQueryParameters queryParameters)
        {
            throw new NotImplementedException();
        }
    }
}
