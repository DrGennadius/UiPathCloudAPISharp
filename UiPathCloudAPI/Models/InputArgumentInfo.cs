using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class InputArgumentInfo : ArgumentInfo
    {
        [JsonProperty(PropertyName = "required")]
        public bool Required { get; set; }

        [JsonProperty(PropertyName = "hasDefault")]
        public bool HasDefault { get; set; }
    }
}
