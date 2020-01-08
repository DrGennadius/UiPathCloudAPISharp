using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    interface IGetRequest<T>
    {
        IEnumerable<T> GetCollection();

        IEnumerable<T> GetCollection(IQueryParameters queryParameters);
    }
}
