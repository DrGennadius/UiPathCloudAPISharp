using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Tests
{
    [TestClass]
    public class ModelMainipulationTests
    {
        private UiPathCloudAPI uiPath;

        [TestInitialize]
        public void Init()
        {
            uiPath = Config.CommonUiPathApi;
        }

        //[TestMethod]
        //public void ProcessTest()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
        //        {
        //            throw new Exception(uiPath.LastErrorMessage, ex);
        //        }
        //        throw ex;
        //    }
        //}

        [TestMethod]
        public void AssetTest()
        {
            string testAssetName1 = Guid.NewGuid().ToString();
            string testAssetName2 = Guid.NewGuid().ToString();
            int testValue1 = 1993;
            int testValue2 = 2020;
            try
            {
                Filter filter = new Filter("Name", testAssetName1);
                var asset = uiPath.AssetManager.GetCollection(filter).FirstOrDefault();
                Assert.IsNull(asset);

                IntegerAsset asset1 = new IntegerAsset(testAssetName1, testValue1);
                uiPath.AssetManager.CreateInstance(asset1);
                asset = uiPath.AssetManager.GetCollection(filter).FirstOrDefault();
                Assert.IsNotNull(asset);

                Assert.AreEqual(asset.Name, testAssetName1);
                asset.Name = testAssetName2;
                asset.Value = testValue2;
                uiPath.AssetManager.ChangeInstance(asset);
                filter = new Filter("Name", testAssetName2);
                asset = uiPath.AssetManager.GetCollection(filter).FirstOrDefault();
                Assert.IsNotNull(asset);
                Assert.AreEqual(asset.Name, testAssetName2);
                Assert.AreEqual(asset.Value, testValue2);

                uiPath.AssetManager.DeleteInstance(asset);
                asset = uiPath.AssetManager.GetCollection(filter).FirstOrDefault();
                Assert.IsNull(asset);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uiPath.LastErrorMessage))
                {
                    throw new Exception(uiPath.LastErrorMessage, ex);
                }
                throw ex;
            }
        }
    }
}
