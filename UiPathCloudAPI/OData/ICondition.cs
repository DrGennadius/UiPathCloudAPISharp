using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.OData
{
    public interface ICondition : IOData
    {
        PrimitiveCondition[] GetPrimitives();
    }
}
