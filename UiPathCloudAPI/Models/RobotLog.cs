using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class RobotLog
    {
        public string Level { get; set; }

        public string WindowsIdentity { get; set; }

        public string ProcessName { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Message { get; set; }

        public string JobKey { get; set; }

        public string RawMessage { get; set; }

        public string RobotName { get; set; }

        public int MachineId { get; set; }

        public int Id { get; set; }
    }
}
