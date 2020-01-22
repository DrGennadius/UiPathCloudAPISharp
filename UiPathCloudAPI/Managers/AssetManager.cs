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
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestExecutor _requestExecutor;

        internal AssetManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public IEnumerable<Asset> GetCollection()
        {
            string response = _requestExecutor.SendRequestGetForOdata("Assets");
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Items;
        }

        public IEnumerable<Asset> GetCollection(string conditions)
        {
            return GetCollection(new Filter(conditions));
        }

        public IEnumerable<Asset> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip));
        }

        public IEnumerable<Asset> GetCollection(IQueryParameters queryParameters)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Assets", queryParameters);
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

        public IEnumerable<ConcreteAsset> GetConcreteCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1)
        {
            return GetCollection(top, filter, select, expand, orderby, skip).Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(IQueryParameters queryParameters)
        {
            return GetCollection(queryParameters).Select(x => x.Concrete()).ToList();
        }

        public Asset GetInstanceByRobotId(string assetName, int robotId)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Assets/UiPath.Server.Configuration.OData.GetRobotAssetByRobotId(robotId={0},assetName='{1}')", robotId, assetName));
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

        public void DeleteInstance(Asset instance)
        {
            _requestExecutor.SendRequestDeleteForOdata(string.Format("Assets({0})", instance.Id));
        }

        public Asset CreateInstance(Asset instance)
        {
            string output = JsonConvert.SerializeObject(instance);
            byte[] sentData = Encoding.UTF8.GetBytes(output);
            string response = _requestExecutor.SendRequestPostForOdata("Assets", sentData);
            return JsonConvert.DeserializeObject<Asset>(response);
        }

        public Asset CreateInstance(ConcreteAsset instance)
        {
            return CreateInstance(instance.Asset);
        }

        public ConcreteAsset CreateInstanceAndReturnConcrete(Asset instance)
        {
            return CreateInstance(instance).Concrete();
        }

        public ConcreteAsset CreateInstanceAndReturnConcrete(ConcreteAsset instance)
        {
            return CreateInstance(instance).Concrete();
        }

        public void ChangeInstance(Asset instance)
        {
            string output = JsonConvert.SerializeObject(instance);
            byte[] sentData = Encoding.UTF8.GetBytes(output);
            _requestExecutor.SendRequestPutForOdata(string.Format("Assets({0})", instance.Id), sentData);
        }

        public int Count()
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Assets", queryParameters);
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Count;
        }
    }
}
