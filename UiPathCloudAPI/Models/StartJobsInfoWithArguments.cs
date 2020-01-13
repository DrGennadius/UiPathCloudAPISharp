using Newtonsoft.Json;
using System.Collections.Generic;

namespace UiPathCloudAPISharp.Models
{
    public class StartJobsInfoWithArguments : StartJobsInfo
    {
        /// <summary>
        /// Representation of input arguments as a single string.
        /// </summary>
        [JsonProperty(PropertyName = "InputArguments")]
        public string InputArgumentsAsString { get; set; }

        /// <summary>
        /// Representation of input arguments as <see cref="Dictionary{string, object}"/>.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> InputArguments
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InputArgumentsAsString))
                {
                    return new Dictionary<string, object>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(InputArgumentsAsString);
                }
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    InputArgumentsAsString = null;
                }
                else
                {
                    InputArgumentsAsString = JsonConvert.SerializeObject(value);
                }
            }
        }
    }
}
