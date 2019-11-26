using Newtonsoft.Json;
using System.Collections.Generic;

namespace UiPathCloudAPISharp.Models
{
    public class BasicArgumentsInfo
    {
        public BasicArgumentsInfo(string input, string output)
        {
            Input = input;
            Output = output;
        }

        public BasicArgumentsInfo()
        {
        }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}
