using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    public class OrderBy : IQueryParameters
    {
        /// <summary>
        /// Order by <paramref name="value"/> and using <paramref name="sort"/>.
        /// </summary>
        /// <param name="value">Value for order</param>
        /// <param name="sort">None, Ascending (asc) or Descending (desc) order</param>
        public OrderBy(string value, Sort sort)
        {
            Value = value;
            Sort = sort;
        }

        /// <summary>
        /// Order by <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value for order</param>
        public OrderBy(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Order by ...
        /// </summary>
        public OrderBy() { }

        /// <summary>
        /// Value for order.
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// None, Ascending (asc) or Descending (desc) order.
        /// </summary>
        public Sort Sort { get; set; } = Sort.None;

        public string GetQueryString()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return "";
            }
            else if (Sort == Sort.None)
            {
                return string.Format("$orderby={0}", Value);
            }
            return string.Format("$orderby={0}%20{1}", Value, Sort.ToString().ToLower());
        }
    }

    /// <summary>
    /// None, Ascending (asc) or Descending (desc) order.
    /// </summary>
    public enum Sort
    {
        None,
        Asc,
        Desc
    }
}
