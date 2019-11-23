using Newtonsoft.Json;

namespace UiPathCloudAPISharp.Models
{
    class StartInfoContainer<T>
    {
        [JsonProperty(PropertyName = "startInfo")]
        public T StartJobsInfo { get; set; }
    }
}
