using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    public interface ICondition : IQueryString
    {
        PrimitiveCondition[] GetPrimitives();
    }
}
