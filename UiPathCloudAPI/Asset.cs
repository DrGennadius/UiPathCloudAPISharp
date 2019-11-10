using Newtonsoft.Json;

namespace UiPathCloudAPISharp
{
    public class Asset
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }
        
        public string Name { get; set; }

        public string ValueType { get; set; }

        public string StringValue { get; set; }

        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        public string CredentialUsername { get; set; }

        public string CredentialPassword { get; set; }
    }
}
