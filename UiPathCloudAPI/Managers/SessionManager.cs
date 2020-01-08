using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Common;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Managers
{
    public class SessionManager : IManager, IGetRequest<SessionManager>
    {
        public QueryStore QueryStore => throw new NotImplementedException();

        public IEnumerable<SessionManager> GetCollection()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SessionManager> GetCollection(IQueryParameters queryParameters)
        {
            throw new NotImplementedException();
        }
    }
}
