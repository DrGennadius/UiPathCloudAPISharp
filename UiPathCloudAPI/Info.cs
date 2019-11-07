using Newtonsoft.Json;
using System.Collections.Generic;

namespace UiPathOrchestrator
{
    public class Info<T>
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "@odata.count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<T> Items { get; set; }
    }
}
