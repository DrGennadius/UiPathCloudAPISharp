using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class NewMultipleQueueItems
    {
        public NewMultipleQueueItems()
        {
            QueueItems = new List<NewQueueItemData>();
        }

        [JsonProperty(PropertyName = "queueName")]
        public string QueueName { get; set; }

        [JsonProperty(PropertyName = "commitType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QueueItemsCommitType CommitType { get; set; }

        [JsonProperty(PropertyName = "queueItems")]
        public List<NewQueueItemData> QueueItems { get; set; }

        public void Add(NewQueueItemData queueItemData)
        {
            QueueItems.Add(queueItemData);
        }

        public void Clear()
        {
            QueueItems.Clear();
        }
    }

    public enum QueueItemsCommitType
    {
        AllOrNothing,
        ProcessAllIndependently
    }
}
