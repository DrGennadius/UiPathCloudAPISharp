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
    public class TransactionTests
    {
        private UiPathCloudAPI uiPath;

        [TestInitialize]
        public void Init()
        {
            uiPath = Config.CommonUiPathApi;
        }

        [TestMethod]
        public void AddQueueItemTest()
        {
            Filter filter = new Filter("Name", "TestQueue");
            var queueDefinition = uiPath.TransactionManager.GetQueueDefinitions(filter).FirstOrDefault();
            if (queueDefinition != null)
            {
                NewQueueItem newQueueItem = new NewQueueItem
                {
                    ItemData = new NewQueueItemData
                    {
                        Name = "TestQueue",
                        Priority = "High"
                    }
                };
                newQueueItem.ItemData.AddSpecificContent("Test1", "Test1");
                newQueueItem.ItemData.AddSpecificContent("Test2", 2);
                newQueueItem.ItemData.AddSpecificContent("Test3", false);
                var queueItem = uiPath.TransactionManager.AddQueueItem(newQueueItem);
                Assert.IsNotNull(queueItem);
            }
        }

        [TestMethod]
        public void AddMultipleQueueItems()
        {
            Filter filter = new Filter("Name", "TestQueue");
            var queueDefinition = uiPath.TransactionManager.GetQueueDefinitions(filter).FirstOrDefault();
            if (queueDefinition != null)
            {
                NewMultipleQueueItems newMultipleQueueItems = new NewMultipleQueueItems
                {
                    QueueName = "TestQueue",
                    CommitType = QueueItemsCommitType.AllOrNothing
                };
                NewQueueItemData newQueueItemData1 = new NewQueueItemData
                {
                    Name = "Test1",
                    Priority = "High"
                };
                newQueueItemData1.AddSpecificContent("Test1", "Test1");
                NewQueueItemData newQueueItemData2 = new NewQueueItemData
                {
                    Name = "Test2",
                    Priority = "High"
                };
                newQueueItemData2.AddSpecificContent("Test2", 2);
                NewQueueItemData newQueueItemData3 = new NewQueueItemData
                {
                    Name = "Test3",
                    Priority = "High"
                };
                newQueueItemData3.AddSpecificContent("Test3", false);
                newMultipleQueueItems.Add(newQueueItemData1);
                newMultipleQueueItems.Add(newQueueItemData2);
                newMultipleQueueItems.Add(newQueueItemData3);
                var transactionStatus1 = uiPath.TransactionManager.AddQueueItems(newMultipleQueueItems);
                
                newQueueItemData1 = new NewQueueItemData
                {
                    Name = "Test1",
                    Priority = "High",
                    Reference = "ref1"
                };
                newQueueItemData1.AddSpecificContent("Test1", "Test1");
                newQueueItemData2 = new NewQueueItemData
                {
                    Name = "Test2",
                    Priority = "High",
                    Reference = "ref2"
                };
                newQueueItemData2.AddSpecificContent("Test2", 2);
                newQueueItemData3 = new NewQueueItemData
                {
                    Name = "Test3",
                    Priority = "High",
                    Reference = "ref3"
                };
                newQueueItemData3.AddSpecificContent("Test3", false);
                newMultipleQueueItems.Clear();
                newMultipleQueueItems.Add(newQueueItemData1);
                newMultipleQueueItems.Add(newQueueItemData2);
                newMultipleQueueItems.Add(newQueueItemData3);
                var transactionStatus2 = uiPath.TransactionManager.AddQueueItems(newMultipleQueueItems);
            }
        }
    }
}
