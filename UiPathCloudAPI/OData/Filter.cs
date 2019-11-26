using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace UiPathCloudAPISharp.OData
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

        public Filter(string name, IODataTransform oDataTransform)
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
            _resultBuilder = new StringBuilder();
        }

        public void Clear()
        {
            _resultBuilder.Clear();
        }

        public void AddCondition(string baseName, string propertyName, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            AddCondition(new Condition(baseName, propertyName, value, comparisonOperator));
        }

        public void AddCondition(string name, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            AddCondition(new Condition(name, value, comparisonOperator));
        }

        public void AddCondition(string name, IODataTransform oDataTransform)
        {
            AppendToResult(oDataTransform.GetODataString(name));
        }

        public void AddCondition(string conditionalExpression)
        {
            Regex regex = new Regex("((?<= )and|or(?= ))");
            string[] elements = regex.Split(conditionalExpression);
            string lastLogicalOperator = "and";
            bool isFirst = true;
            foreach (var item in elements)
            {
                string lowItem = item.ToLower();
                bool isLogicalOperator = lowItem == "and" || lowItem == "or";
                if (isLogicalOperator && isFirst || isLogicalOperator && !string.IsNullOrEmpty(lastLogicalOperator))
                {
                    throw new ArgumentException("Condition string is incorrected:\n\"{0}\"", conditionalExpression);
                }
                if (isLogicalOperator)
                {
                    lastLogicalOperator = lowItem;
                }
                else
                {
                    AddCondition(new Condition(item), GetLogicalOperator(lastLogicalOperator));
                    lastLogicalOperator = null;
                }
                isFirst = false;
            }
            if (!string.IsNullOrEmpty(lastLogicalOperator))
            {
                throw new ArgumentException("Condition string is incorrected:\n\"{0}\"", conditionalExpression);
            }
        }

        public void AddCondition(ICondition condition, LogicalOperator logicalOperator)
        {
            AppendToResult(condition.GetODataString(), logicalOperator.ToString().ToLower());
        }

        public void AddCondition(ICondition condition)
        {
            AppendToResult(condition.GetODataString());
        }

        public string GetODataString()
        {
            return "$filter=" + _resultBuilder.ToString();
        }

        public override string ToString()
        {
            return GetODataString();
        }

        private StringBuilder _resultBuilder;

        private void AppendToResult(string condition, string logicalOperator = "and")
        {
            if (_resultBuilder.Length > 0)
            {
                if (string.IsNullOrWhiteSpace(logicalOperator))
                {
                    logicalOperator = "and";
                }
                _resultBuilder.Append(string.Format("%20{0}%20{1}", logicalOperator, condition));
            }
            else
            {
                _resultBuilder.Append(condition);
            }
        }

        private LogicalOperator GetLogicalOperator(string logicalOperator)
        {
            if (logicalOperator.ToLower() == "and")
            {
                return LogicalOperator.And;
            }
            else if (logicalOperator.ToLower() == "or")
            {
                return LogicalOperator.Or;
            }
            else
            {
                throw new Exception("Unknow logical operator");
            }
        }
    }

    public enum LogicalOperator
    {
        And,
        Or
    }
}
