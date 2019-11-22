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
        
        public void Clear()
        {
            _resultBuilder.Clear();
        }

        public void AddCondition(Type objectType, string propertyName, object objectValue, ConditionOp condition = ConditionOp.EQ)
        {
            CheckProperty(objectType, propertyName, objectValue);
            AddCondition(objectType.Name, propertyName, objectValue.ToString(), condition);
        }

        public void AddCondition(Type objectType, string propertyName, string objectValue, ConditionOp condition = ConditionOp.EQ)
        {
            CheckProperty(objectType, propertyName, objectValue);
            AddCondition(objectType.Name, propertyName, objectValue, condition);
        }

        public void AddCondition(Type objectType, string propertyName, DateTimeRange dateTimeRange)
        {
            CheckProperty(objectType, propertyName, typeof(DateTime));
            AddCondition(objectType.Name, propertyName, dateTimeRange);
        }
        
        public void AddCondition(string objectName, string propertyName, string objectValue, ConditionOp condition = ConditionOp.EQ)
        {
            string objectFullName = objectName + '/' + propertyName;
            AddCondition(objectFullName, objectValue, condition);
        }

        public void AddCondition(string objectName, string objectValue, ConditionOp condition = ConditionOp.EQ)
        {
            AddCondition(condition.ToString(), objectName, objectValue);
        }

        public void AddCondition(string objectName, string propertyName, DateTimeRange dateTimeRange)
        {
            string objectFullName = objectName + '/' + propertyName;
            AddCondition(objectFullName, dateTimeRange);
        }

        public void AddCondition(string objectName, DateTimeRange dateTimeRange)
        {
            AppendToResult(dateTimeRange.GetString(objectName));
        }

        public void AddCondition(string objectName, string objectValue, string condition)
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

        private void CheckProperty(Type objectType, string propertyName, DateTimeRange dateTimeRange)
        {
            CheckTypes(CheckProperty(objectType, propertyName), typeof(DateTime));
        }

        private void CheckProperty(Type objectType, string propertyName, Type valueType)
        {
            CheckTypes(CheckProperty(objectType, propertyName), valueType);
        }

        private void CheckProperty(Type objectType, string propertyName, object objectValue)
        {
            CheckTypes(CheckProperty(objectType, propertyName), objectValue.GetType());
        }

        private PropertyInfo CheckProperty(Type objectType, string propertyName)
        {
            var propertyInfo = objectType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException("The property is not found.");
            }
            return propertyInfo;
        }

        private void CheckTypes(PropertyInfo propertyInfo, Type valueType)
        {
            CheckTypes(propertyInfo.PropertyType, valueType);
        }

        private void CheckTypes(Type propertyType, Type valueType)
        {
            if (propertyType != valueType)
            {
                throw new ArgumentException("Property type and value type for this property are different.");
            }
        }

        public string GetODataString()
        {
            return _resultBuilder.ToString();
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
