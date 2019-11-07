using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathOrchestratorStartJob.UiPath
{
    public class Session
    {
        public string HostMachineName { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string State { get; set; }

        public string ReportingTime { get; set; }

        public string Info { get; set; }

        public bool IsUnresponsive { get; set; }

        public string LicenseErrorCode { get; set; }

        public int OrganizationUnitId { get; set; }

        public string FolderName { get; set; }

        public int Id { get; set; }
    }
}
