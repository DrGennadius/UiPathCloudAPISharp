﻿using System;
using System.Text;

namespace UiPathCloudAPISharp.Query
{
    public class Interval<T> : IQueryStringTransform where T : struct, IComparable<T>
    {
        public Interval(T start, T end)
            : this()
        {
            Start = start;
            End = end;
        }

        public Interval()
        {
            IncludeStart = true;
            IncludeEnd = true;
        }

        public T? Start { get; set; }

        public T? End { get; set; }

        public bool IncludeStart { get; set; }

        public bool IncludeEnd { get; set; }

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

        public string GetQueryString(string input)
        {
            PrimitiveCondition[] primitiveConditions = GeneratePrimitiveConditions(input);

            if (primitiveConditions.Length == 1)
            {
                return primitiveConditions[0].GetQueryString();
            }
            else if (primitiveConditions.Length == 2)
            {
                return string.Format("{0}%20and%20{1}", primitiveConditions[0].GetQueryString(), primitiveConditions[1].GetQueryString());
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
                    primitiveConditions = new PrimitiveCondition[] { new PrimitiveCondition(input, GetNormalizeValue(Start.Value), ComparisonOperator.NE) };
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
                return new PrimitiveCondition(valueName, GetNormalizeValue(Start.Value), GetComparisonOperatorForStart());
            }
            return null;
        }

        private PrimitiveCondition GeneratePrimitiveConditionForEnd(string valueName)
        {
            if (End.HasValue)
            {
                return new PrimitiveCondition(valueName, GetNormalizeValue(End.Value), GetComparisonOperatorForEnd());
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

        private ComparisonOperator GetComparisonOperatorForStart()
        {
            if (IncludeStart)
            {
                return ComparisonOperator.GE;
            }
            else
            {
                return ComparisonOperator.GT;
            }
        }

        private ComparisonOperator GetComparisonOperatorForEnd()
        {
            if (IncludeEnd)
            {
                return ComparisonOperator.LE;
            }
            else
            {
                return ComparisonOperator.LT;
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