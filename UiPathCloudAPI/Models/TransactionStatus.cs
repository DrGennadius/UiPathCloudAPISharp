using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class TransactionStatus
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        [JsonProperty(PropertyName = "FailedItems")]
        public List<string> FailedItemStrings { get; set; }
    }
}
