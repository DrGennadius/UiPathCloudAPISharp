using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Common
{
    public class CloudConfiguration : UiPathConfigurationBase
    {
        public override ConnectionType ConnectionType
        {
            get
            {
                return ConnectionType.Cloud;
            }
        }

        public string TenantLogicalName { get; set; }

        public string ClientId { get; set; }

        public string UserKey { get; set; }

        public string AccountLogicalName { get; set; }
    }
}
