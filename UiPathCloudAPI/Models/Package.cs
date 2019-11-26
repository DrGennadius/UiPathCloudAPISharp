using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class Package : Library
    {
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "Arguments")]
        public BasicArgumentsInfo BasicArguments { get; set; }

        public ArgumentsInfo Arguments
        {
            get
            {
                return new ArgumentsInfo(BasicArguments);
            }
            set
            {
                BasicArguments = value.GetBaseArguments();
            }
        }
    }
}
