using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp
{
    public interface ICondition : IOData
    {
        PrimitiveCondition[] GetPrimitives();
    }
}
