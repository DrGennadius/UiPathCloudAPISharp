using Newtonsoft.Json;

namespace UiPathCloudAPISharp
{
    public class Account
    {
        [JsonProperty(PropertyName = "accountName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "accountLogicalName")]
        public string LogicalName { get; set; }
    }
}
