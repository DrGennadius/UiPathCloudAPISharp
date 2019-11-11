using System.Collections.Generic;

namespace UiPathCloudAPISharp
{
    public class StartJobsWithArgumentsInfo : StartJobsInfo
    {
        public Dictionary<string, object> InputArguments { get; set; }
    }
}
