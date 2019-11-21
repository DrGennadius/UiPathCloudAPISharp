using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class CommonFilter : IFilter
    {
        public CommonFilter()
        {
            _resultBuilder = new StringBuilder();
        }

        public string Value
        {
            get
            {
                return _resultBuilder.ToString();
            }
        }

        public void Clear()
        {
            _resultBuilder.Clear();
        }

        public void AddCondition(ConditionOp condition, Type objectType, string propertyName, object objectValue)
        {
            var propertyInfo = objectType.GetProperty(propertyName);
            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType == objectValue.GetType())
                {
                    AddCondition(condition, objectType.Name, propertyName, objectValue.ToString());
                }
                else
                {
                    throw new ArgumentException("Property type and value type for this property are different.");
                }
            }
            else
            {
                throw new ArgumentException("The property is not found.");
            }
        }

        public void AddCondition(ConditionOp condition, Type objectType, string propertyName, string objectValue)
        {
            AddCondition(condition, objectType.Name, propertyName, objectValue);
        }

        public void AddCondition(ConditionOp condition, string objectName, string propertyName, string objectValue)
        {
            string objectFullName = objectName + '/' + propertyName;
            AddCondition(condition, objectFullName, objectValue);
        }

        public void AddCondition(string condition, object objectBase, object objectValue)
        {
            string objectName = "";
            if (objectBase == null)
            {
                throw new ArgumentNullException("object is null");
            }
            if (objectBase.GetType().IsClass)
            {
                objectName = objectBase.GetType().Name;
            }
            else if (objectBase.ToString() == "System.RuntimeType")
            {
                objectName = objectBase.ToString();
            }
            else
            {
                throw new ArgumentException("object has incorect type");
            }
            AddCondition(condition, "", "");
        }

        public void AddCondition(ConditionOp condition, string objectName, string objectValue)
        {
            AddCondition(condition.ToString(), objectName, objectValue);
        }

        public void AddCondition(string condition, string objectName, string objectValue)
        {
            AppendToResult(string.Format("{0}%20{1}%20%27{2}%27", objectName, condition, objectValue));
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

    public enum ConditionOp
    {
        /// <summary>
        /// Equal
        /// </summary>
        EQ,

        /// <summary>
        /// Not equal
        /// </summary>
        NE,

        /// <summary>
        /// Greater than
        /// </summary>
        GT,

        /// <summary>
        /// Greater than or equal
        /// </summary>
        GE,

        /// <summary>
        /// Less than
        /// </summary>
        LT,

        /// <summary>
        /// Less than or equal
        /// </summary>
        LE
    }
}
