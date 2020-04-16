using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Common
{
    public abstract class UiPathConfigurationBase
    {
        public BehaviorMode BehaviorMode { get; set; }

        public string BaseURL { get; set; }

        public string AccountAuthURL { get; set; }

        public abstract ConnectionType ConnectionType { get; }
    }

    public enum ConnectionType
    {
        /// <summary>
        /// Last connection type.
        /// </summary>
        Cloud,

        /// <summary>
        /// Old connection type (dont using).
        /// </summary>
        OnPremise
    }
}
