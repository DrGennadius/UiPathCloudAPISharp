using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp
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

        public string GetODataString()
        {
            return Interval.GetODataString(Name);
        }

        public PrimitiveCondition[] GetPrimitives()
        {
            return Interval.GeneratePrimitiveConditions(Name);
        }
    }
}
