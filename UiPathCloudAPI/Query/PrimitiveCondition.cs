using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    public class PrimitiveCondition : ICondition
    {
        public PrimitiveCondition(string name, string value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
            : this()
        {
            Name = name;
            Value = value;
            ComparisonOperator = comparisonOperator;
        }

        public PrimitiveCondition()
        {
            ComparisonOperator = Query.ComparisonOperator.EQ;
        }

        /// <summary>
        /// Element name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Element value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Condition operation
        /// </summary>
        public ComparisonOperator ComparisonOperator { get; set; }

        public string GetQueryString()
        {
            return string.Format("{0}%20{1}%20{2}", Name, ComparisonOperator.ToString().ToLower(), Value);
        }

        public PrimitiveCondition[] GetPrimitives()
        {
            return new PrimitiveCondition[] { this };
        }
    }

    public enum ComparisonOperator
    {
        /// <summary>
        /// Equal
        /// </summary>
        EQ,

        /// <summary>
        /// Not equal
        /// </summary>
        NE,

        /// <summary>
        /// Greater than
        /// </summary>
        GT,

        /// <summary>
        /// Greater than or equal
        /// </summary>
        GE,

        /// <summary>
        /// Less than
        /// </summary>
        LT,

        /// <summary>
        /// Less than or equal
        /// </summary>
        LE
    }
}
