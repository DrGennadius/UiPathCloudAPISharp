using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace UiPathCloudAPISharp.Query
{
    public class Condition : ICondition
    {
        public Condition(Type objectType, string propertyName, object objectValue, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
            : this(comparisonOperator)
        {
            SetNameAndValue(objectType, propertyName, objectValue);
        }

        public Condition(Type objectClass, object objectValue, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
            : this(comparisonOperator)
        {
            SetNameAndValue(objectClass, objectValue);
        }

        public Condition(string baseName, string propertyName, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
            : this(comparisonOperator)
        {
            BaseName = baseName;
            PropertyName = propertyName;
            Value = value;
        }

        public Condition(string name, object value, ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
            : this(comparisonOperator)
        {
            Name = name;
            Value = value;
        }

        public Condition(string condition)
            : this()
        {
            Set(condition);
        }

        public Condition(ComparisonOperator comparisonOperator = ComparisonOperator.EQ)
        {
            ComparisonOperator = comparisonOperator;
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
        public ComparisonOperator ComparisonOperator { get; set; }

        public string GetQueryString()
        {
            string name = Name;
            if (!string.IsNullOrEmpty(name))
            {
                return string.Format("{0}%20{1}%20{2}", Name, ComparisonOperator.ToString().ToLower(), GetNormalizeValue(Value));
            }
            return "";
        }

        public PrimitiveCondition[] GetPrimitives()
        {
            return new PrimitiveCondition[] { new PrimitiveCondition(Name, GetNormalizeValue(Value), ComparisonOperator) };
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

        public void Set(string condition)
        {
            Regex regex = new Regex("(!=|=|>=|<=|>|<)");
            string[] elements = regex.Split(condition);
            if (elements.Count() == 3)
            {
                regex = new Regex("\\w+");
                var matches = regex.Matches(elements[0]);
                if (matches.Count == 1)
                {
                    object value = TryCast(elements[2]);
                    if (value != null)
                    {
                        Value = value;
                        Name = matches[0].Value;
                        ComparisonOperator = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), GetODataComparisonOperator(elements[1]).ToUpper());
                    }
                    else
                    {
                        throw new ArgumentException("Incorrect the condition string.");
                    }
                }
                else
                {
                    throw new ArgumentException("Incorrect the condition string.");
                }
            }
            else
            {
                throw new ArgumentException("Incorrect the condition string.");
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Name, ComparisonOperatorToString(), ValueToString());
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

        private string GetNormalizeValue(object value)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else if (value is string)
            {
                return "%27" + value.ToString() + "%27";
            }
            else
            {
                return value.ToString();
            }
        }

        private object TryCast(string value)
        {
            object obj = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                string obviouslyString = TryCastObviouslyString(value);
                if (!string.IsNullOrEmpty(obviouslyString))
                {
                    obj = obviouslyString;
                }
                else
                {
                    try
                    {
                        obj = Convert.ToInt32(value);
                    }
                    catch { }
                    if (obj == null)
                    {
                        try
                        {
                            obj = Convert.ToBoolean(value);
                        }
                        catch { }
                    }
                    if (obj == null)
                    {
                        obj = value;
                    }
                }
            }
            return obj;
        }

        private string TryCastObviouslyString(string value)
        {
            string result = null;

            Regex regex = new Regex("(?<=').*(?=')");
            var match = regex.Match(value);
            if (match.Success)
            {
                result = match.Value;
            }

            return result;
        }

        private string ComparisonOperatorToString()
        {
            if (ComparisonOperator == ComparisonOperator.EQ)
            {
                return "=";
            }
            else if (ComparisonOperator == ComparisonOperator.NE)
            {
                return "!=";
            }
            else if (ComparisonOperator == ComparisonOperator.GE)
            {
                return ">=";
            }
            else if (ComparisonOperator == ComparisonOperator.GT)
            {
                return ">";
            }
            else if (ComparisonOperator == ComparisonOperator.LE)
            {
                return "<=";
            }
            else if (ComparisonOperator == ComparisonOperator.LT)
            {
                return "<";
            }
            return "";
        }

        private string GetODataComparisonOperator(string comparisonOperator)
        {
            if (comparisonOperator == "=")
            {
                return "eq";
            }
            else if (comparisonOperator == "!=")
            {
                return "ne";
            }
            else if (comparisonOperator == ">=")
            {
                return "ge";
            }
            else if (comparisonOperator == ">")
            {
                return "gt";
            }
            else if (comparisonOperator == "<=")
            {
                return "le";
            }
            else if (comparisonOperator == "<")
            {
                return "lt";
            }
            return "";
        }

        private string ValueToString()
        {
            string result = "";

            if (Value is string && !string.IsNullOrEmpty(Value as string))
            {
                result = "'" + Value as string + "'";
            }
            else
            {
                result = Value.ToString();
            }

            return result;
        }
    }
}
