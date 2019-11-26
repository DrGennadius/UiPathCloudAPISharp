using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class Schedule : ScheduleBase
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }

        public bool Enabled { get; set; }

        public string ReleaseKey { get; set; }

        public string PackageName { get; set; }

        public string EnvironmentName { get; set; }

        public string EnvironmentId { get; set; }

        public string StartProcessCronSummary { get; set; }

        public string StartProcessNextOccurrence { get; set; }
        
        public string ExternalJobKey { get; set; }
        
        public string TimeZoneIana { get; set; }

        public bool UseCalendar { get; set; }

        public string StopProcessDateReleaseName { get; set; }

        public string InputArguments { get; set; }

        public int Id { get; set; }
    }
}
