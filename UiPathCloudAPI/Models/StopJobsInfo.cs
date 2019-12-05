using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class StopJobsInfo
    {
        [JsonProperty(PropertyName = "strategy")]
        public string StrategyNumber { get; set; }

        [JsonProperty(PropertyName = "jobIds")]
        public int[] JobIds { get; set; }

        [JsonIgnore]
        public StopJobsStrategy Strategy
        {
            get
            {
                return (StopJobsStrategy)Enum.Parse(typeof(StopJobsStrategy), StrategyNumber);
            }
            set
            {
                StrategyNumber = value.ToString();
            }
        }
    }

    public enum StopJobsStrategy
    {
        SoftStop = 1,
        Kill = 2
    }
}
