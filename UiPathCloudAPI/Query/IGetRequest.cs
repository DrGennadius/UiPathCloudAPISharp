using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    interface IGetRequest<T>
    {
        IEnumerable<T> GetCollection();

        IEnumerable<T> GetCollection(string conditions);

        IEnumerable<T> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1);

        IEnumerable<T> GetCollection(IQueryParameters queryParameters);

        T GetInstance(int id);

        T GetInstance(T instance);

        int Count();
    }
}
