using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class NewQueueItem
    {
        [JsonProperty(PropertyName = "itemData")]
        public NewQueueItemData ItemData { get; set; }
    }
}
