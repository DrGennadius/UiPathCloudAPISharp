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

        public IEnumerable<Asset> GetCollection(Folder folder)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Assets", folder);
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Items;
        }

        public IEnumerable<Asset> GetCollection(string conditions, Folder folder = null)
        {
            return GetCollection(new Filter(conditions), folder);
        }

        public IEnumerable<Asset> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(new QueryParameters(top, filter, select, expand, orderby, skip), folder);
        }

        public IEnumerable<Asset> GetCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata("Assets", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Items;
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection()
        {
            return GetCollection().Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(Folder folder)
        {
            return GetCollection(folder).Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(string conditions, Folder folder = null)
        {
            return GetCollection(conditions, folder).Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null)
        {
            return GetCollection(top, filter, select, expand, orderby, skip, folder).Select(x => x.Concrete()).ToList();
        }

        public IEnumerable<ConcreteAsset> GetConcreteCollection(IQueryParameters queryParameters, Folder folder = null)
        {
            return GetCollection(queryParameters, folder).Select(x => x.Concrete()).ToList();
        }

        public Asset GetInstanceByRobotId(string assetName, int robotId, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Assets/UiPath.Server.Configuration.OData.GetRobotAssetByRobotId(robotId={0},assetName='{1}')", robotId, assetName), folder);
            return JsonConvert.DeserializeObject<Asset>(response);
        }

        public Asset GetInstanceByRobot(string assetName, Robot robot, Folder folder = null)
        {
            return GetInstanceByRobotId(assetName, robot.Id, folder);
        }

        public ConcreteAsset GetConcreteInstanceByRobotId(string assetName, int robotId, Folder folder = null)
        {
            return GetInstanceByRobotId(assetName, robotId, folder).Concrete();
        }

        public ConcreteAsset GetConcreteInstanceByRobot(string assetName, Robot robot, Folder folder = null)
        {
            return GetConcreteInstanceByRobotId(assetName, robot.Id, folder);
        }

        public void DeleteInstance(Asset instance, Folder folder = null)
        {
            _requestExecutor.SendRequestDeleteForOdata(string.Format("Assets({0})", instance.Id), folder);
        }

        public Asset CreateInstance(Asset instance, Folder folder = null)
        {
            string output = JsonConvert.SerializeObject(instance);
            byte[] sentData = Encoding.UTF8.GetBytes(output);
            string response = _requestExecutor.SendRequestPostForOdata("Assets", sentData, folder);
            return JsonConvert.DeserializeObject<Asset>(response);
        }

        public Asset CreateInstance(ConcreteAsset instance, Folder folder = null)
        {
            return CreateInstance(instance.Asset, folder);
        }

        public ConcreteAsset CreateInstanceAndReturnConcrete(Asset instance, Folder folder = null)
        {
            return CreateInstance(instance, folder).Concrete();
        }

        public ConcreteAsset CreateInstanceAndReturnConcrete(ConcreteAsset instance, Folder folder = null)
        {
            return CreateInstance(instance, folder).Concrete();
        }

        public void ChangeInstance(Asset instance, Folder folder = null)
        {
            string output = JsonConvert.SerializeObject(instance);
            byte[] sentData = Encoding.UTF8.GetBytes(output);
            _requestExecutor.SendRequestPutForOdata(string.Format("Assets({0})", instance.Id), sentData, folder);
        }

        public int Count(Folder folder = null)
        {
            QueryParameters queryParameters = new QueryParameters(top: 0);
            string response = _requestExecutor.SendRequestGetForOdata("Assets", queryParameters, folder);
            return JsonConvert.DeserializeObject<Info<Asset>>(response).Count;
        }
    }
}
