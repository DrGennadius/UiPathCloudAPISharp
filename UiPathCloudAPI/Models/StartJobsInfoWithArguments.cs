using Newtonsoft.Json;
using System.Collections.Generic;

namespace UiPathCloudAPISharp.Models
{
    public class StartJobsInfoWithArguments : StartJobsInfo
    {
        [JsonProperty(PropertyName = "InputArguments")]
        public string InputArgumentsAsString { get; set; }

        [JsonIgnore]
        public Dictionary<string, object> InputArguments
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(InputArgumentsAsString);
            }
            set
            {
                InputArgumentsAsString = JsonConvert.SerializeObject(value);
            }
        }
    }
}
