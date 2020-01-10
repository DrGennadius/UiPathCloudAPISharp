using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class ConfigurationInfo
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }
        
        public string Scope { get; set; }
        
        public List<Setting> Configuration { get; set; }
    }
}
