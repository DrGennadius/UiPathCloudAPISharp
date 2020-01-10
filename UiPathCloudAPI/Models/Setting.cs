using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class Setting
    {
        public string Key { get; set; }

        public string DisplayName { get; set; }

        public string ValueType { get; set; }

        public string DefaultValue { get; set; }

        public string[] PossibleValues { get; set; }
    }
}
