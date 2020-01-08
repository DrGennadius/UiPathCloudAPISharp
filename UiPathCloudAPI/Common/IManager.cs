using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Common
{
    interface IManager
    {
        QueryStore QueryStore { get; }
    }
}
