using System;
using System.Text;

namespace UiPathCloudAPISharp.OData
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
            PrimitiveCondition[] primitiveConditions = GeneratePrimitiveConditions(input);

            if (primitiveConditions.Length == 1)
            {
                return primitiveConditions[0].GetODataString();
            }
            else if (primitiveConditions.Length == 2)
            {
                return string.Format("{0}%20and%20{1}", primitiveConditions[0].GetODataString(), primitiveConditions[1].GetODataString());
            }

            return "";
        }

        public PrimitiveCondition[] GeneratePrimitiveConditions(string input)
        {
            PrimitiveCondition[] primitiveConditions = new PrimitiveCondition[0];
            if (Start.HasValue && End.HasValue && Start.Value.CompareTo(End.Value) == 0)
            {
                if (IncludeStart && IncludeEnd)
                {
                    primitiveConditions = new PrimitiveCondition[] { new PrimitiveCondition(input, GetNormalizeValue(Start.Value)) };
                }
                else if (!IncludeStart && !IncludeEnd)
                {
                    primitiveConditions = new PrimitiveCondition[] { new PrimitiveCondition(input, GetNormalizeValue(Start.Value), ConditionOperation.NE) };
                }
            }
            else
            {
                PrimitiveCondition startCondition = GeneratePrimitiveConditionForStart(input);
                PrimitiveCondition endCondition = GeneratePrimitiveConditionForEnd(input);
                if (startCondition != null && endCondition != null)
                {
                    primitiveConditions = new PrimitiveCondition[] { startCondition, endCondition };
                }
                else if (startCondition != null && endCondition == null)
                {
                    primitiveConditions = new PrimitiveCondition[] { startCondition };
                }
                else if (startCondition == null && endCondition != null)
                {
                    primitiveConditions = new PrimitiveCondition[] { endCondition };
                }
            }
            return primitiveConditions;
        }

        public string GetNormalizeStart()
        {
            if (Start.HasValue)
            {
                return GetNormalizeValue(Start.Value);
            }
            return "";
        }

        public string GetNormalizeEnd()
        {
            if (End.HasValue)
            {
                return GetNormalizeValue(End.Value);
            }
            return "";
        }

        private PrimitiveCondition GeneratePrimitiveConditionForStart(string valueName)
        {
            if (Start.HasValue)
            {
                return new PrimitiveCondition(valueName, GetNormalizeValue(Start.Value), GetConditionOperationForStart());
            }
            return null;
        }

        private PrimitiveCondition GeneratePrimitiveConditionForEnd(string valueName)
        {
            if (End.HasValue)
            {
                return new PrimitiveCondition(valueName, GetNormalizeValue(End.Value), GetConditionOperationForEnd());
            }
            return null;
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

        private ConditionOperation GetConditionOperationForStart()
        {
            if (IncludeStart)
            {
                return ConditionOperation.GE;
            }
            else
            {
                return ConditionOperation.GT;
            }
        }

        private ConditionOperation GetConditionOperationForEnd()
        {
            if (IncludeEnd)
            {
                return ConditionOperation.LE;
            }
            else
            {
                return ConditionOperation.LT;
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