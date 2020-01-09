using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp
{
    /// <summary>
    /// The behavior mode affects the logic of initialization, authorization, and call requests.
    /// </summary>
    public enum BehaviorMode
    {
        /// <summary>
        /// No use automatic initiation and authorization.
        /// </summary>
        Default,

        /// <summary>
        /// Automatic initiation when trying to execute a request if not yet authorized or timeout token life.
        /// </summary>
        AutoInitiation,

        /// <summary>
        /// Automatic authorization when trying to execute a request if not yet authorized or timeout token life.
        /// </summary>
        AutoAuthorization
    }
}
