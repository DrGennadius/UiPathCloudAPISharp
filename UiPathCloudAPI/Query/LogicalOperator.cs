using System;

namespace UiPathCloudAPISharp.Query
{
    public class LogicalOperator : IQueryString
    {
        public LogicalOperator(LogicalOperatorType logicalOperator)
        {
            Value = logicalOperator;
        }

        public LogicalOperator(string lowItem)
        {
            Value = GetLogicalOperator(lowItem);
        }

        public LogicalOperatorType Value { get; set; }

        public string GetQueryString()
        {
            return Value.ToString().ToLower();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static LogicalOperator And
        {
            get
            {
                return new LogicalOperator(LogicalOperatorType.And);
            }
        }

        public static LogicalOperator Or
        {
            get
            {
                return new LogicalOperator(LogicalOperatorType.Or);
            }
        }

        private LogicalOperatorType GetLogicalOperator(string logicalOperator)
        {
            if (logicalOperator.ToLower() == "and")
            {
                return LogicalOperatorType.And;
            }
            else if (logicalOperator.ToLower() == "or")
            {
                return LogicalOperatorType.Or;
            }
            else
            {
                throw new Exception("Unknow logical operator");
            }
        }
    }

    public enum LogicalOperatorType
    {
        And,
        Or
    }
}