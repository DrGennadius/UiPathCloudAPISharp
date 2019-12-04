using Newtonsoft.Json;

namespace UiPathCloudAPISharp.Models
{
    public class AuthParameters
    {
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType => "refresh_token";

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }
    }
}
