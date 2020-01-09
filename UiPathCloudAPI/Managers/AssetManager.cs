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
    public class AssetManager : IManager
    {
        public QueryStore QueryStore => throw new NotImplementedException();

        private RequestManager _requestManager;

        internal AssetManager(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        public IEnumerable<Asset> GetCollection()
        {
            string response = _requestManager.SendRequestGetForOdata("Assets");
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Items;
        }

        public IEnumerable<Asset> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Asset> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Asset> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestManager.SendRequestGetForOdata("Assets", queryParameters);
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Items;
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection()
        {
            return GetCollection().Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(string conditions)
        {
            return GetCollection(conditions).Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            return GetCollection(top, filter, select, expand, orderby, skip).Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(IQueryParameters queryParameters)
        {
            return GetCollection(queryParameters).Select(x => x.Concrete()).ToList();
        }

        public Asset GetInstanceByRobotId(string assetName, int robotId)
        {
            string response = _requestManager.SendRequestGetForOdata(string.Format("Assets/UiPath.Server.Configuration.OData.GetRobotAssetByRobotId(robotId={0},assetName='{1}')", robotId, assetName));
            return JsonConvert.DeserializeObject<Asset>(response);
        }

        public Asset GetInstanceByRobot(string assetName, Robot robot)
        {
            return GetInstanceByRobotId(assetName, robot.Id);
        }

        public ConcreteAsset GetConcreteInstanceByRobotId(string assetName, int robotId)
        {
            return GetInstanceByRobotId(assetName, robotId).Concrete();
        }

        public ConcreteAsset GetConcreteInstanceByRobot(string assetName, Robot robot)
        {
            return GetConcreteInstanceByRobotId(assetName, robot.Id);
        }
    }
}
