using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UiPathCloudAPISharp.OData
{
    public class Filter : IFilter
    {
        public Filter(string baseName, string propertyName, object value, ConditionOperation conditionOperation = ConditionOperation.EQ)
            : this()
        {
            AddCondition(baseName, propertyName, value, conditionOperation);
        }

        public Filter(string name, object value, ConditionOperation conditionOperation = ConditionOperation.EQ)
            : this()
        {
            AddCondition(name, value, conditionOperation);
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

        public void AddCondition(string baseName, string propertyName, object value, ConditionOperation conditionOperation = ConditionOperation.EQ)
        {
            AddCondition(new Condition(baseName, propertyName, value, conditionOperation));
        }

        public void AddCondition(string name, object value, ConditionOperation conditionOperation = ConditionOperation.EQ)
        {
            AddCondition(new Condition(name, value, conditionOperation));
        }

        public void AddCondition(string condition)
        {
            AddCondition(new Condition(condition));
        }

        public void AddCondition(ICondition condition)
        {
            AppendToResult(condition.GetODataString());
        }

        public string GetODataString()
        {
            return "$filter=" + _resultBuilder.ToString();
        }

        private StringBuilder _resultBuilder;

        private void AppendToResult(string element)
        {
            if (_resultBuilder.Length > 0)
            {
                _resultBuilder.Append(string.Format("%20and%20", element));
            }
            else
            {
                _resultBuilder.Append(element);
            }
        }
    }
}
