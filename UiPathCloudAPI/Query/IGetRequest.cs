using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Models;

namespace UiPathCloudAPISharp.Query
{
    interface IGetRequest<T>
    {
        IEnumerable<T> GetCollection();

        IEnumerable<T> GetCollection(Folder folder);
        
        IEnumerable<T> GetCollection(string conditions, Folder folder = null);

        IEnumerable<T> GetCollection(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, int skip = -1, Folder folder = null);
        
        IEnumerable<T> GetCollection(IQueryParameters queryParameters, Folder folder = null);
        
        T GetInstance(int id, Folder folder = null);
        
        T GetInstance(T instance, Folder folder = null);
        
        int Count(Folder folder = null);
    }
}
