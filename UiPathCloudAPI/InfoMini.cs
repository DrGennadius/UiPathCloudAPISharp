using Newtonsoft.Json;
using System.Collections.Generic;

namespace UiPathOrchestrator
{
    public class InfoMini<T>
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "value")]
        public List<T> Items { get; set; }
    }
}
