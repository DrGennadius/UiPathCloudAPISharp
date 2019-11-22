using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class Condition : ICondition
    {
        public Condition(Type objectType, string propertyName, object objectValue, ConditionOperation conditionOperation = ConditionOperation.EQ)
        {
            SetNameAndValue(objectType, propertyName, objectValue);
            ConditionOperation = conditionOperation;
        }

        public Condition(Type objectClass, object objectValue, ConditionOperation conditionOperation = ConditionOperation.EQ)
        {
            SetNameAndValue(objectClass, objectValue);
            ConditionOperation = conditionOperation;
        }

        public Condition(string baseName, string propertyName, object value, ConditionOperation conditionOperation = ConditionOperation.EQ)
        {
            BaseName = baseName;
            PropertyName = propertyName;
            Value = value;
            ConditionOperation = conditionOperation;
        }

        public Condition(string name, object value, ConditionOperation conditionOperation = ConditionOperation.EQ)
        {
            Name = name;
            Value = value;
            ConditionOperation = conditionOperation;
        }

        public Condition()
        {
        }

        /// <summary>
        /// Element base name
        /// </summary>
        public string BaseName { get; set; }

        /// <summary>
        /// Element property Name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Element full name
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(PropertyName))
                {
                    return BaseName;
                }
                return BaseName + '/' + PropertyName;
            }
            set
            {
                string[] names = value.Split('/');
                if (names.Count() == 1)
                {
                    BaseName = value;
                }
                else if (names.Count() == 2)
                {
                    BaseName = names[0];
                    PropertyName = names[1];
                }
                else
                {
                    throw new Exception(string.Format("Incorrected Name = {0}", value));
                }
            }
        }

        /// <summary>
        /// Element value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Condition operation
        /// </summary>
        public ConditionOperation ConditionOperation { get; set; } = ConditionOperation.EQ;

        public string GetODataString()
        {
            return string.Format("{0}%20{1}%20%27{2}%27", Name, ConditionOperation.ToString().ToLower(), Value);
        }

        public PrimitiveCondition[] GetPrimitives()
        {
            return new PrimitiveCondition[] { new PrimitiveCondition(Name, Value.ToString(), ConditionOperation) };
        }

        public void SetNameAndValue(Type objectType, string propertyName, object objectValue)
        {
            CheckProperty(objectType, propertyName, objectValue);
            BaseName = objectType.Name;
            PropertyName = propertyName;
        }

        public void SetNameAndValue(Type objectClass, object objectValue)
        {
            CheckTypes(objectClass, objectValue.GetType());
            BaseName = objectClass.Name;
        }

        private void CheckProperty(Type objectType, string propertyName, Interval<DateTime> dateTimeInterval)
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
    }
}
