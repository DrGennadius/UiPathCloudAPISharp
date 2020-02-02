using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class QueueDefinition
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int MaxNumberOfRetries { get; set; }

        public bool AcceptAutomaticallyRetry { get; set; }

        public bool EnforceUniqueReference { get; set; }

        public DateTime CreationTime { get; set; }

        public int Id { get; set; }
    }
}
