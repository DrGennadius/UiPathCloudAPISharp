using Newtonsoft.Json;
namespace UiPathCloudAPISharp
{
    class StartInfoContainer<T>
    {
        [JsonProperty(PropertyName = "startInfo")]
        public T StartJobsInfo { get; set; }
    }
}
