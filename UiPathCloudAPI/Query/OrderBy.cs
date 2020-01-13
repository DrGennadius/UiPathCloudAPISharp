using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    public class OrderBy : IQueryParameters
    {
        /// <summary>
        /// Order by <paramref name="value"/> and using <paramref name="sortDirection"/>.
        /// </summary>
        /// <param name="value">Value for order</param>
        /// <param name="sortDirection">None, Ascending (asc) or Descending (desc) order</param>
        public OrderBy(string value, SortDirection sortDirection)
        {
            Value = value;
            SortDirection = sortDirection;
        }

        /// <summary>
        /// Order by <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value for order</param>
        public OrderBy(string value)
            : this(value, SortDirection.None)
        {
        }

        /// <summary>
        /// Order by ...
        /// </summary>
        public OrderBy() : this("", SortDirection.None) { }

        /// <summary>
        /// Value for order.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// None, Ascending (asc) or Descending (desc) order.
        /// </summary>
        public SortDirection SortDirection { get; set; }

        public string GetQueryString()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return "";
            }
            else if (SortDirection == SortDirection.None)
            {
                return string.Format("$orderby={0}", Value);
            }
            return string.Format("$orderby={0}%20{1}", Value, SortDirection.ToString().ToLower());
        }
    }

    /// <summary>
    /// None, Ascending (asc) or Descending (desc) order.
    /// </summary>
    public enum SortDirection
    {
        None,
        Asc,
        Desc
    }
}
