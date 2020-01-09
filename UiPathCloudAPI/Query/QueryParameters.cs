using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    public class QueryParameters : IQueryParameters
    {
        public QueryParameters(int top = -1, IFilter filter = null, string select = null, string expand = null, OrderBy orderby = null, string skip = null)
        {
            Top = top;
            Filter = filter;
            Select = select;
            Expand = expand;
            OrderBy = orderby;
            Skip = Skip;
        }

        public int Top { get; set; } = -1;

        public string Select { get; set; }

        public string Expand { get; set; }

        public OrderBy OrderBy { get; set; }

        public string Skip { get; set; }

        public IFilter Filter { get; set; }

        private StringBuilder _resultBuilder;

        private void AppendToResult(string element)
        {
            if (_resultBuilder.Length > 0)
            {
                _resultBuilder.Append(string.Format("&{0}", element));
            }
            else
            {
                _resultBuilder.Append(element);
            }
        }
        
        public string GetQueryString()
        {
            if (_resultBuilder == null)
            {
                _resultBuilder = new StringBuilder();
            }
            else
            {
                _resultBuilder.Clear();
            }

            if (Top != -1)
            {
                AppendToResult(string.Format("$top={0}", Top));
            }
            string filterString = Filter?.GetQueryString();
            if (!string.IsNullOrEmpty(filterString))
            {
                AppendToResult(filterString);
            }
            if (!string.IsNullOrEmpty(Select))
            {
                AppendToResult(string.Format("$select={0}", Select));
            }
            if (!string.IsNullOrEmpty(Expand))
            {
                AppendToResult(string.Format("$expand={0}", Expand));
            }
            string orderByString = OrderBy?.GetQueryString();
            if (!string.IsNullOrEmpty(orderByString))
            {
                AppendToResult(orderByString);
            }
            if (!string.IsNullOrEmpty(Skip))
            {
                AppendToResult(string.Format("skip={0}", Skip));
            }

            return _resultBuilder.ToString();
        }

        public override string ToString()
        {
            return GetQueryString();
        }
    }
}
