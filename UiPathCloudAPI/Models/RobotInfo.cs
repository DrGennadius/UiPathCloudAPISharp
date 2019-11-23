using System;

namespace UiPathCloudAPISharp.Models
{
    /// <summary>
    /// Extended information about the robot
    /// </summary>
    public class RobotInfo
    {
        public string State { get; set; }

        public DateTime ReportingTime { get; set; }

        public string Info { get; set; }

        public bool IsUnresponsive { get; set; }

        public int Id { get; set; }

        public Robot Robot { get; set; }
    }
}
