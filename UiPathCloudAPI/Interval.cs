using System;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class Interval<T> : IODataTransform where T : struct, IComparable<T>
    {
        public Interval(T start, T end)
        {
            Start = start;
            End = end;
        }

        public Interval()
        {
        }

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

        public Type GetValueType()
        {
            return typeof(T);
        }

        public string GetODataString(string input)
        {
            string startPart = "", endPart = "";

            if (Start.HasValue && End.HasValue && Start.Value.CompareTo(End.Value) == 0)
            {
                if (IncludeStart && IncludeEnd)
                {
                    startPart = string.Format("{0}%20eq%20{1}", input, GetNormalizeValue(Start.Value));
                }
                else if (!IncludeStart && !IncludeEnd)
                {
                    startPart = string.Format("{0}%20ne%20{1}", input, GetNormalizeValue(Start.Value));
                }
            }
            else
            {
                startPart = GetStartPart(input);
                endPart = GetEndPart(input);
            }
            if (!string.IsNullOrEmpty(startPart) && !string.IsNullOrEmpty(endPart))
            {
                startPart += "%20and%20";
            }

            return startPart + endPart;
        }

        private string GetStartPart(string valueName)
        {
            if (Start.HasValue)
            {
                return string.Format("{0}%20{1}%20{2}", valueName, GetOperationForStart(), GetNormalizeValue(Start.Value));
            }
            return "";
        }

        private string GetEndPart(string valueName)
        {
            if (End.HasValue)
            {
                return string.Format("{0}%20{1}%20{2}", valueName, GetOperationForEnd(), GetNormalizeValue(End.Value));
            }
            return "";
        }

        private string GetOperationForStart()
        {
            if (IncludeStart)
            {
                return "ge";
            }
            else
            {
                return "gt";
            }
        }

        private string GetOperationForEnd()
        {
            if (IncludeEnd)
            {
                return "le";
            }
            else
            {
                return "lt";
            }
        }

        private string GetNormalizeValue(T value)
        {
            if (typeof(T) == typeof(DateTime))
            {
                DateTime? dateTime = value as DateTime?;
                return dateTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else
            {
                return value.ToString();
            }
        }
    }
}