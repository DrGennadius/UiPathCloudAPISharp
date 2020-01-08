using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    public class IntervalCondition<T> : ICondition where T : struct, IComparable<T>
    {
        public IntervalCondition(string name, Interval<T> interval)
        {
            Name = name;
            Interval = interval;
        }

        public IntervalCondition()
        {
        }

        /// <summary>
        /// Element name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Interval for conditions
        /// </summary>
        public Interval<T> Interval { get; set; }

        public string GetQueryString()
        {
            return Interval.GetQueryString(Name);
        }

        public PrimitiveCondition[] GetPrimitives()
        {
            return Interval.GeneratePrimitiveConditions(Name);
        }
    }
}
