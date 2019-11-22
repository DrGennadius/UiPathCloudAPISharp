using System;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class Interval<T> where T : struct, IComparable<T>
    {
        public T? Start { get; set; }

        public T? End { get; set; }

        public bool IncludeStart { get; set; } = true;

        public bool IncludeEnd { get; set; } = true;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(IncludeStart ? "[" : "(");
            stringBuilder.Append(Start.HasValue ? Start.Value.ToString() : "Infinity");
            stringBuilder.Append(':');
            stringBuilder.Append(End.HasValue ? End.Value.ToString() : "Infinity");
            stringBuilder.Append(IncludeEnd ? "]" : ")");
            return stringBuilder.ToString();
        }

        public bool IsValid()
        {
            if (Start.HasValue && End.HasValue)
            {
                return Start.Value.CompareTo(End.Value) <= 0;
            }
            return true;
        }

        public bool ContainsValue(T value)
        {
            bool gStart = false, lEnd = false;
            if (Start.HasValue)
            {
                if (IncludeStart && Start.Value.CompareTo(value) <= 0 || Start.Value.CompareTo(value) < 0)
                {
                    gStart = true;
                }
            }
            else
            {
                gStart = true;
            }
            if (gStart)
            {
                if (End.HasValue)
                {
                    if (IncludeStart && value.CompareTo(End.Value) <= 0 || value.CompareTo(End.Value) < 0)
                    {
                        lEnd = true;
                    }
                }
                else
                {
                    lEnd = true;
                }
            }
            return gStart && lEnd;
        }

        public bool IsInsideInterval(Interval<T> interval)
        {
            return IsValid() && interval.IsValid() && interval.ContainsValue(Start.Value) && interval.ContainsValue(End.Value);
        }

        public bool ContainsInterval(Interval<T> interval)
        {
            return IsValid() && interval.IsValid() && ContainsValue(interval.Start.Value) && ContainsValue(interval.End.Value);
        }
    }
}