using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class Machine
    {
        public string LicenseKey { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int NonProductionSlots { get; set; }

        public int UnattendedSlots { get; set; }

        public int Id { get; set; }

        public object RobotVersions { get; set; }
    }
}
