using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class JobWithArguments : Job, IContainerWithArguments
    {
        /// <summary>
        /// Representation of input arguments as a single string.
        /// </summary>
        [JsonProperty(PropertyName = "InputArguments")]
        public string InputArgumentsAsString { get; set; }

        /// <summary>
        /// Representation of output arguments as a single string.
        /// </summary>
        [JsonProperty(PropertyName = "OutputArguments")]
        public string OutputArgumentsAsString { get; set; }

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

        /// <summary>
        /// Representation of output arguments as <see cref="Dictionary{string, object}"/>.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> OutputArguments
        {
            get
            {
                if (string.IsNullOrWhiteSpace(OutputArgumentsAsString))
                {
                    return new Dictionary<string, object>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(OutputArgumentsAsString);
                }
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    OutputArgumentsAsString = null;
                }
                else
                {
                    OutputArgumentsAsString = JsonConvert.SerializeObject(value);
                }
            }
        }

        /// <summary>
        /// Get input and output arguments as dictionaries.
        /// </summary>
        /// <returns></returns>
        public Arguments GetArguments()
        {
            return new Arguments(InputArguments, OutputArguments);
        }
    }
}
