using Newtonsoft.Json;
namespace UiPathOrchestrator
{
    class StartInfoContainer<T>
    {
        [JsonProperty(PropertyName = "startInfo")]
        public T StartJobsInfo { get; set; }
    }
}
