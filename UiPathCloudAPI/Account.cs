using Newtonsoft.Json;

namespace UiPathOrchestrator
{
    public class Account
    {
        [JsonProperty(PropertyName = "accountName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "accountLogicalName")]
        public string LogicalName { get; set; }
    }
}
