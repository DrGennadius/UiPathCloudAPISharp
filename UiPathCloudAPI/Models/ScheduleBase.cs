using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class ScheduleBase
    {
        public string Name { get; set; }

        public int ReleaseId { get; set; }

        public string ReleaseName { get; set; }

        public string StartProcessCron { get; set; }

        public string StartProcessCronDetails { get; set; }

        public int StartStrategy { get; set; }

        public string ExecutorRobots { get; set; }

        public string StopProcessExpression { get; set; }

        public string StopStrategy { get; set; }

        public string TimeZoneId { get; set; }
    }
}
