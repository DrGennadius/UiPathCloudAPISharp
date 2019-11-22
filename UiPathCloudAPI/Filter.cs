using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class Filter : IFilter
    {
        public Filter()
        {
            _resultBuilder = new StringBuilder();
        }
        
        public void Clear()
        {
            _resultBuilder.Clear();
        }

        public void AddCondition(ICondition condition)
        {
            AppendToResult(condition.GetODataString());
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

        public string GetODataString()
        {
            return _resultBuilder.ToString();
        }
    }
}
