using Newtonsoft.Json;

namespace UiPathCloudAPISharp
{
    public class ServiceInstance
    {
        [JsonProperty(PropertyName = "serviceInstanceName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "serviceInstanceLogicalName")]
        public string LogicalName { get; set; }

        [JsonProperty(PropertyName = "serviceType")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "serviceUrl")]
        public string Url { get; set; }
    }
}
