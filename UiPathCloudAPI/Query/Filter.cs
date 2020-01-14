using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace UiPathCloudAPISharp.Query
{
    public class Filter : IFilter
    {
        public Filter(string baseName, string propertyName, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
            : this()
        {
            AddCondition(baseName, propertyName, value, comparisonOperator);
        }

        public Filter(string name, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
            : this()
        {
            AddCondition(name, value, comparisonOperator);
        }

        public Filter(string name, IQueryStringTransform oDataTransform)
            : this()
        {
            AddCondition(name, oDataTransform);
        }

        public Filter(string name, DateTime start, DateTime end)
            : this()
        {
            Interval<DateTime> interval = new Interval<DateTime>(start, end);
            AddCondition(name, interval);
        }

        public Filter(ICondition condition)
            : this()
        {
            AddCondition(condition);
        }

        public Filter(string condition)
            : this()
        {
            AddCondition(condition);
        }

        public Filter()
        {
            ConditionLine = new List<IQueryString>();
        }

        public List<IQueryString> ConditionLine { get; private set; }

        public void AddCondition(string baseName, string propertyName, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            AddCondition(new Condition(baseName, propertyName, value, comparisonOperator));
        }

        public void AddCondition(string name, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            AddCondition(new Condition(name, value, comparisonOperator));
        }

        public void AddCondition(string name, IQueryStringTransform oDataTransform)
        {
            AddCondition(oDataTransform.GetQueryString(name));
        }

        public void AddCondition(string conditionalExpression)
        {
            Regex regex = new Regex(@"((?<=\s*)and|or(?=\s*))");
            string[] elements = regex.Split(conditionalExpression);
            bool isFirst = true;
            foreach (var item in elements)
            {
                string lowItem = item.ToLower();
                bool isLogicalOperator = lowItem == "and" || lowItem == "or";
                if (isLogicalOperator && isFirst || isLogicalOperator && ConditionLine.LastOrDefault() is LogicalOperator)
                {
                    throw new ArgumentException("Condition string is incorrected:\n\"{0}\"", conditionalExpression);
                }
                if (isLogicalOperator)
                {
                    ConditionLine.Add(new LogicalOperator(lowItem));
                }
                else
                {
                    AddCondition(new Condition(item));
                }
                isFirst = false;
            }
            if (ConditionLine.LastOrDefault() is LogicalOperator)
            {
                throw new ArgumentException("Condition string is incorrected:\n\"{0}\"", conditionalExpression);
            }
        }

        public void AddCondition(ICondition condition, LogicalOperatorType logicalOperator)
        {
            ConditionLine.Add(condition);
            ConditionLine.Add(new LogicalOperator(logicalOperator));
        }

        public void AddCondition(ICondition condition)
        {
            if (ConditionLine.LastOrDefault() is ICondition)
            {
                ConditionLine.Add(LogicalOperator.And);
            }
            ConditionLine.Add(condition);
        }

        public string GetQueryString()
        {
            int conditionCount = ConditionLine.Count;
            if (conditionCount == 0)
            {
                return "";
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < conditionCount; i++)
                {
                    stringBuilder.Append(ConditionLine[i].GetQueryString());
                    if (i < conditionCount - 1)
                    {
                        stringBuilder.Append("%20");
                    }
                }
                return "$filter=" + stringBuilder.ToString();
            }
        }

        public override string ToString()
        {
            return GetQueryString();
        }
    }
}
